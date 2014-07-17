using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAttributeValue
        : RDomBase<IAttributeValue, ISymbol>, IAttributeValue
    {
        //private LiteralKind _literalKind;
        //private object _value;
        //private Type _type;
        //private AttributeValueStyle _style;

        public RDomAttributeValue(
            SyntaxNode rawItem,
            SemanticModel model)
            : base(rawItem, model)
        {
            //Initialize(model);
            //var tuple = GetAttributeValueName(rawItem, attributeSyntax);
            //Name = tuple.Item1;
            //_style = tuple.Item2;
        }

        internal RDomAttributeValue(
             RDomAttributeValue oldRDom)
            : base(oldRDom)
        {
            ValueType = oldRDom.ValueType;
            Value = oldRDom.Value;
            // TODO: manage type
           Type = oldRDom.Type;
            Style = oldRDom.Style;
        }

        //protected void Initialize(SemanticModel model)
        //{
        //    base.Initialize();
        //    var tuple = GetAttributeValueValue(TypedSyntax, model);
        //    _value = tuple.Item1;
        //    _literalKind = tuple.Item2;
        //    _type = _value.GetType();
        //}

        //private Tuple<object, LiteralKind> GetAttributeValueValue(
        //            SyntaxNode argNode, SemanticModel model)
        //{
        //    var arg = argNode as AttributeArgumentSyntax;
        //    if (arg == null) throw new InvalidOperationException();
        //    // TODO: Manage multiple values because of AllowMultiples, param array, or missing symbol 
        //    var expr = arg.Expression;
        //    var literalKind = LiteralKind.Unknown;
        //    object value = null;
        //    var literalExpression = expr as LiteralExpressionSyntax;
        //    if (literalExpression != null)
        //    { value = GetLiteralValue(literalExpression, ref literalKind); }
        //    else
        //    {
        //        var typeExpression = expr as TypeOfExpressionSyntax;
        //        if (typeExpression != null)
        //        {
        //            literalKind = LiteralKind.Type;
        //            value = GetTypeExpressionValue(typeExpression, model);
        //        }
        //    }
        //    return new Tuple<object, LiteralKind>(value, literalKind);
        //}

        //private object GetTypeExpressionValue(TypeOfExpressionSyntax typeExpression, SemanticModel model)
        //{
        //    object value = null;
        //    var typeSyntax = typeExpression.Type;
        //    var predefinedTypeSyntax = typeSyntax as PredefinedTypeSyntax;
        //    var identifierNameSyntax = typeSyntax as IdentifierNameSyntax;
        //    if (predefinedTypeSyntax != null)
        //    {
        //        var typeInfo = model.GetTypeInfo(predefinedTypeSyntax);
        //        value = new RDomReferencedType(typeInfo, null);
        //    }
        //    else if (identifierNameSyntax != null)
        //    {
        //        var typeInfo = model.GetTypeInfo(identifierNameSyntax);
        //        value = new RDomReferencedType(typeInfo, null);
        //    }
        //    else
        //    {
        //        // I don't know how to get here, but if I get here, I want to know it :)
        //        throw new InvalidOperationException();
        //    }
        //    return value;
        //}

        //private object GetLiteralValue(LiteralExpressionSyntax literalExpression, ref LiteralKind literalKind)
        //{
        //    literalKind = RoslynUtilities.LiteralKindFromSyntaxKind(literalExpression.Token.CSharpKind());
        //    return literalExpression.Token.Value;
        //}

        //private static Tuple<string, AttributeValueStyle> GetAttributeValueName(
        //   AttributeArgumentSyntax arg, AttributeSyntax attributeSyntax)
        //{
        //    string name;
        //    AttributeValueStyle style;
        //    if (arg.NameColon != null)
        //    {
        //        style = AttributeValueStyle.Colon;
        //        name = arg.NameColon.Name.ToString().Replace(":", "").Trim();
        //    }
        //    else if (arg.NameEquals != null)
        //    {
        //        style = AttributeValueStyle.Equals;
        //        name = arg.NameEquals.Name.ToString();
        //    }
        //    else
        //    {
        //        style = AttributeValueStyle.Positional;
        //        name = attributeSyntax.Name.ToString();
        //    }
        //    return new Tuple<string, AttributeValueStyle>(name, style);
        //}

        public object Value { get; set; }
       
        public LiteralKind ValueType { get; set; }

        public AttributeValueStyle Style { get; set; }

        public Type Type { get; set; }
        //internal static IEnumerable<IAttributeValue> MakeAttributeValues(
        //        AttributeSyntax attrib, RDomBase attributedItem, SemanticModel model)
        //{
        //    var ret = new List<IAttributeValue>();
        //    if (attrib.ArgumentList != null)
        //    {
        //        var arguments = attrib.ArgumentList.Arguments;
        //        foreach (AttributeArgumentSyntax arg in arguments)
        //        {
        //            ret.Add(new RDomAttributeValue(arg, attrib, attributedItem, model));
        //        }
        //    }
        //    return ret;
        //}

        //public  AttributeArgumentSyntax BuildSyntax()
        //{
        //    var argNameSyntax = SyntaxFactory.IdentifierName(Name);
        //    var kind = RoslynUtilities.SyntaxKindFromLiteralKind(_literalKind, _value);
        //    ExpressionSyntax expr = null;
        //    if (_literalKind == LiteralKind.Boolean) { expr = SyntaxFactory.LiteralExpression(kind); }
        //    else
        //    {
        //        var methodInfo = ReflectionUtilities.FindMethod(typeof(SyntaxFactory), "Literal", _type);
        //        if (methodInfo == null) throw new InvalidOperationException();
        //        var token = (SyntaxToken)methodInfo.Invoke(null, new object[] { _value });
        //        expr = SyntaxFactory.LiteralExpression(kind, token);
        //    }

        //    if (_style == AttributeValueStyle.Colon )
        //    {
        //        return SyntaxFactory.AttributeArgument(
        //            null,
        //            SyntaxFactory.NameColon(argNameSyntax), 
        //            expr);
        //    }
        //    else if (_style == AttributeValueStyle.Positional )
        //    {
        //        return SyntaxFactory.AttributeArgument( expr);
        //    }
        //    else 
        //    {
        //        return SyntaxFactory.AttributeArgument(
        //            SyntaxFactory.NameEquals(argNameSyntax),
        //            null, expr);
        //    }
        //}
    }
}
