using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class AttributeFactory : RDomMiscFactory<IAttribute, AttributeSyntax>
    {
        //public override FactoryPriority Priority
        //{ get { return FactoryPriority.Normal; } }
        public AttributeFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        public override RDomPriority Priority
        { get { return 0; } }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return (syntaxNode is AttributeListSyntax || syntaxNode is AttributeSyntax);
        }

        protected override IEnumerable<IMisc> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return InternalCreateFrom(syntaxNode, parent, model);
        }

        private IEnumerable<IAttribute> InternalCreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntaxAsList = syntaxNode as AttributeListSyntax;
            if (syntaxAsList != null) { return CreateFromList(syntaxAsList, parent, model); }
            var attributeSyntax = syntaxNode as AttributeSyntax;
            if (attributeSyntax != null) { return new IAttribute[] { CreateFromItem(attributeSyntax, parent, model) }; }
            return ExtractAttributes(syntaxNode, parent, model);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            return BuildSyntax((IAttribute)item);
        }

        private IEnumerable<SyntaxNode> BuildSyntax(IAttribute item)
        {
            var itemAsT = item as IAttribute;
            var nameSyntax = SyntaxFactory.ParseName(itemAsT.Name);
            var arguments = new SeparatedSyntaxList<AttributeArgumentSyntax>();
            var values = itemAsT.AttributeValues.Select(x => BuildAttributeValueSyntax(x));
            arguments = arguments.AddRange(values.OfType<AttributeArgumentSyntax>().ToList());
            var argumentList = SyntaxFactory.AttributeArgumentList(arguments);
            var node = SyntaxFactory.Attribute(nameSyntax, argumentList);
            var nodeList = SyntaxFactory.AttributeList(
                                SyntaxFactory.SeparatedList(
                                    new AttributeSyntax[] {
                                        (AttributeSyntax)BuildSyntaxHelpers.PrepareForBuildItemSyntaxOutput(node, item)
                                    }));

            return new SyntaxNode[] { nodeList };
        }

        //public IEnumerable<SyntaxNode> BuildSyntax(AttributeList attributeList)
        //{
        //    var list = SyntaxFactory.List<SyntaxNode>();
        //    var attributes = attributeList.Attributes;
        //    if (attributes.Any())
        //    {
        //        var attribList = SyntaxFactory.AttributeList();
        //        var attributeSyntaxItems = attributes.SelectMany(x => BuildSyntax(x)).ToArray();
        //        // TODO: attributeSyntaxItems = attributeSyntaxItems.Select(x => x.Format()).ToArray();
        //        attribList = attribList.AddAttributes(attributeSyntaxItems.OfType<AttributeSyntax>().ToArray());
        //        list = list.Add(attribList);
        //    }
        //    return list;
        //}

        public IEnumerable<IAttribute> ExtractAttributes(SyntaxNode parentNode, IDom newParent, SemanticModel model)
        {
            var parentAsHasSymbol = newParent as IRoslynHasSymbol;
            if (parentAsHasSymbol == null) { throw new InvalidOperationException(); }
            var parentSymbol = parentAsHasSymbol.Symbol;
            var symbolAttributes = parentSymbol.GetAttributes();
            var list = new List<IAttribute>();
            foreach (var attributeData in symbolAttributes)
            {
                // TODO: In those cases where we do have a symbol reference to the attribute, try to use it
                var appRef = attributeData.ApplicationSyntaxReference;
                var attribSyntax = parentNode.SyntaxTree.GetRoot().FindNode(appRef.Span) as AttributeSyntax;
                var newItem = CreateFromItem(attribSyntax, newParent, model);
                list.Add(newItem);
            }
            return list;
        }


        #region Private methods to support build syntax
        private AttributeArgumentSyntax BuildAttributeValueSyntax(IAttributeValue atttributeValue)
        {
            var argNameSyntax = SyntaxFactory.IdentifierName(atttributeValue.Name);
            var kind = Mappings.SyntaxKindFromLiteralKind(atttributeValue.ValueType, atttributeValue.Value);
            ExpressionSyntax expr = BuildArgValueExpression(atttributeValue, kind);
            var node = SyntaxFactory.AttributeArgument(expr);
            if (atttributeValue.Style == AttributeValueStyle.Colon)
            { node = node.WithNameColon(SyntaxFactory.NameColon(argNameSyntax)); }
            else if (atttributeValue.Style == AttributeValueStyle.Equals)
            { node = node.WithNameEquals(SyntaxFactory.NameEquals(argNameSyntax)); }
            return node;
        }

        private static ExpressionSyntax BuildArgValueExpression(IAttributeValue atttributeValue, SyntaxKind kind)
        {
            ExpressionSyntax expr = null;
            if (atttributeValue.ValueType == LiteralKind.Boolean)
            { expr = SyntaxFactory.LiteralExpression(kind); }
            else if (atttributeValue.ValueType == LiteralKind.Type)
            { expr = SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(atttributeValue.Value.ToString())); }
            else
            {
                var token = BuildSyntaxHelpers.GetTokenFromKind(atttributeValue.ValueType, atttributeValue.Value);
                expr = SyntaxFactory.LiteralExpression((SyntaxKind)kind, token);
            }

            return expr;
        }

        #endregion

        #region Private methods to support adding attributes
        private IAttribute CreateFromItem(AttributeSyntax attributeSyntax, IDom parent, SemanticModel model)
        {
            var newItem = new RDomAttribute(attributeSyntax, parent, model);
            newItem.Name = attributeSyntax.Name.ToString();
            var values = MakeAttributeValues(attributeSyntax, newItem, model);
            foreach (var value in values)
            { newItem.AddOrMoveAttributeValue(value); }
            return newItem;
        }

        private IEnumerable<IAttribute> CreateFromList(AttributeListSyntax syntaxAsList, IDom parent, SemanticModel model)
        {
            var list = new List<IAttribute>();
            foreach (var attSyntax in syntaxAsList.Attributes)
            { list.Add(CreateFromItem(attSyntax, parent, model)); }
            return list;
        }

        private IEnumerable<IAttributeValue> MakeAttributeValues(
             AttributeSyntax attrib, IDom parent, SemanticModel model)
        {
            var ret = new List<IAttributeValue>();
            if (attrib.ArgumentList != null)
            {
                var arguments = attrib.ArgumentList.Arguments;
                foreach (AttributeArgumentSyntax arg in arguments)
                {
                    var newAttributeValue = new RDomAttributeValue(arg, parent, model);
                    InitializeAttributeValue(newAttributeValue, arg, model);
                    ret.Add(newAttributeValue);
                }
            }
            return ret;
        }

        private void InitializeAttributeValue(
            IAttributeValue newItem, AttributeArgumentSyntax rawItem, SemanticModel model)
        {
            var tuple = GetAttributeValueName(rawItem);
            newItem.Name = tuple.Item1;
            newItem.Style = tuple.Item2;
            var tuple2 = GetAttributeValueValue(rawItem, model);
            newItem.Value = tuple2.Item1;
            newItem.ValueType = tuple2.Item2;
            newItem.Type = newItem.Value.GetType();
        }

        private Tuple<string, AttributeValueStyle> GetAttributeValueName(AttributeArgumentSyntax arg)
        {
            string name = "";
            AttributeValueStyle style;
            if (arg.NameColon != null)
            {
                style = AttributeValueStyle.Colon;
                name = arg.NameColon.Name.ToString().Replace(":", "").Trim();
            }
            else if (arg.NameEquals != null)
            {
                style = AttributeValueStyle.Equals;
                name = arg.NameEquals.Name.ToString();
            }
            else
            {
                style = AttributeValueStyle.Positional;
                // TODO: Work harder at getting the real parameter name
                //name = attributeSyntax.Name.ToString();
            }
            return Tuple.Create(name, style);
        }

        private Tuple<object, LiteralKind> GetAttributeValueValue(
                      SyntaxNode argNode, SemanticModel model)
        {
            var arg = argNode as AttributeArgumentSyntax;
            Guardian.Assert.IsNotNull(arg, nameof(arg));

            // TODO: Manage multiple values because of AllowMultiples, param array, or missing symbol 
            var expr = arg.Expression;
            var literalKind = LiteralKind.Unknown;
            object value = null;
            var literalExpression = expr as LiteralExpressionSyntax;
            if (literalExpression != null)
            { value = GetLiteralValue(literalExpression, ref literalKind); }
            else
            {
                var typeExpression = expr as TypeOfExpressionSyntax;
                if (typeExpression != null)
                {
                    literalKind = LiteralKind.Type;
                    value = GetTypeExpressionValue(typeExpression, model);
                }
            }
            return Tuple.Create(value, literalKind);
        }

        private object GetTypeExpressionValue(TypeOfExpressionSyntax typeExpression, SemanticModel model)
        {
            object value = null;
            var typeSyntax = typeExpression.Type;
            var predefinedTypeSyntax = typeSyntax as PredefinedTypeSyntax;
            var identifierNameSyntax = typeSyntax as IdentifierNameSyntax;
            if (predefinedTypeSyntax != null)
            {
                var typeInfo = model.GetTypeInfo(predefinedTypeSyntax);
                value = new RDomReferencedType(typeInfo, null);
            }
            else if (identifierNameSyntax != null)
            {
                var typeInfo = model.GetTypeInfo(identifierNameSyntax);
                value = new RDomReferencedType(typeInfo, null);
            }
            else
            {
                // I don't know how to get here, but if I get here, I want to know it :)
                throw new InvalidOperationException();
            }
            return value;
        }

        private object GetLiteralValue(LiteralExpressionSyntax literalExpression, ref LiteralKind literalKind)
        {
            literalKind = Mappings.LiteralKindFromSyntaxKind(literalExpression.Token.CSharpKind());
            return literalExpression.Token.Value;
        }


    }
    #endregion
}
