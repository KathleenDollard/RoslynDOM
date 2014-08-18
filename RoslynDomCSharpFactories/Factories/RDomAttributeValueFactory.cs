using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom.CSharp
{
    public class RDomAttributeValueMiscFactory
            : RDomMiscFactory<RDomAttributeValue, AttributeArgumentSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomAttributeValueMiscFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
            if (_whitespaceLookup == null)
            {
                _whitespaceLookup = new WhitespaceKindLookup();
                _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as AttributeArgumentSyntax;
            var newItem = new RDomAttributeValue(syntaxNode, parent, model);
            InitializeAttributeValue(newItem, syntax, model);
            CreateFromWorker.StandardInitialize(newItem, syntax, parent, model);
            AttributeValueWhitespace(newItem, syntax);
            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IAttributeValue;

            var argNameSyntax = SyntaxFactory.IdentifierName(itemAsT.Name);
            argNameSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirst(argNameSyntax, item.Whitespace2Set[LanguageElement.AttributeValueName]);
            argNameSyntax = BuildSyntaxHelpers.AttachWhitespaceToLast(argNameSyntax, item.Whitespace2Set[LanguageElement.AttributeValueName]);

            var kind = Mappings.SyntaxKindFromLiteralKind(itemAsT.ValueType, itemAsT.Value);
            ExpressionSyntax expr = BuildArgValueExpression(itemAsT, kind);
            var node = SyntaxFactory.AttributeArgument(expr);
            if (itemAsT.Style == AttributeValueStyle.Colon)
            {
                var nameColon = SyntaxFactory.NameColon(argNameSyntax);
                nameColon = BuildSyntaxHelpers.AttachWhitespaceToLast(nameColon, item.Whitespace2Set[LanguageElement.AttributeValueEqualsOrColon]);
                node = node.WithNameColon(nameColon);
            }
            else if (itemAsT.Style == AttributeValueStyle.Equals)
            {
                var nameEquals =SyntaxFactory.NameEquals(argNameSyntax);
                nameEquals = BuildSyntaxHelpers.AttachWhitespaceToLast(nameEquals, item.Whitespace2Set[LanguageElement.AttributeValueEqualsOrColon]);
                node = node.WithNameEquals(nameEquals);
            }
            node = BuildSyntaxHelpers.AttachWhitespaceToFirst(node, item.Whitespace2Set[LanguageElement.AttributeValueFirstToken]);
            node = BuildSyntaxHelpers.AttachWhitespaceToLast(node, item.Whitespace2Set[LanguageElement.AttributeValueLastToken]);

            return node.PrepareForBuildSyntaxOutput(item);
        }

        private void InitializeAttributeValue(IAttributeValue newItem,
                  AttributeArgumentSyntax rawItem, SemanticModel model)
        {
            var tuple = GetAttributeValueName(rawItem);
            newItem.Name = tuple.Item1;
            newItem.Style = tuple.Item2;
            var tuple2 = GetAttributeValueValue(rawItem, newItem, model);
            newItem.Value = tuple2.Item1;
            newItem.ValueType = tuple2.Item2;
            newItem.Type = newItem.Value.GetType();
        }

        private void AttributeValueWhitespace(RDomAttributeValue newItem, AttributeArgumentSyntax syntax)
        {
            CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.GetFirstToken(), LanguagePart.Current, LanguageElement.AttributeValueFirstToken);
            CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.GetLastToken(), LanguagePart.Current, LanguageElement.AttributeValueLastToken);
            if (syntax.NameColon != null)
            {
                CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.NameColon.Name.Identifier, LanguagePart.Current, LanguageElement.AttributeValueName);
                CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.NameColon.ColonToken, LanguagePart.Current, LanguageElement.AttributeValueEqualsOrColon);
            }
            else if (syntax.NameEquals != null)
            {
                CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.NameEquals.Name.Identifier, LanguagePart.Current, LanguageElement.AttributeValueName);
                CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.NameEquals.EqualsToken, LanguagePart.Current, LanguageElement.AttributeValueEqualsOrColon);
            }

            var prevNodeOrToken = syntax.Parent
                                    .ChildNodesAndTokens()
                                    .PreviousSiblings(syntax)
                                    .LastOrDefault();
            if (prevNodeOrToken.CSharpKind() == SyntaxKind.CommaToken)
            {
                var commaToken = prevNodeOrToken.AsToken();
                var whitespace2 = newItem.Whitespace2Set[LanguageElement.AttributeValueFirstToken];
                if (string.IsNullOrEmpty(whitespace2.LeadingWhitespace))
                { whitespace2.LeadingWhitespace = commaToken.TrailingTrivia.ToString(); }
            }
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
                      SyntaxNode argNode, IDom newItem, SemanticModel model)
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
                    value = GetTypeExpressionValue(typeExpression, newItem, model);
                }
            }
            return Tuple.Create(value, literalKind);
        }

        private object GetTypeExpressionValue(TypeOfExpressionSyntax typeExpression, IDom newItem, SemanticModel model)
        {
            var returnType = Corporation
                 .CreateFrom<IMisc>(typeExpression.Type, newItem, model)
                 .FirstOrDefault()
                 as IReferencedType;
            return returnType;

        }

        private object GetLiteralValue(LiteralExpressionSyntax literalExpression, ref LiteralKind literalKind)
        {
            literalKind = Mappings.LiteralKindFromSyntaxKind(literalExpression.Token.CSharpKind());
            return literalExpression.Token.Value;
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


    }

}
