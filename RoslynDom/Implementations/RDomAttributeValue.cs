using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAttributeValue
        : RDomBase<IAttributeValue, AttributeArgumentSyntax, ISymbol>, IAttributeValue
    {
        private string _name;
        private LiteralType _literalType;
        private object _value;

        internal RDomAttributeValue(
            AttributeArgumentSyntax rawItem,
            AttributeSyntax attributeSyntax,
            RDomBase attributedItem)
            : base(rawItem)
        {
            _name = GetAttributeValueName(rawItem, attributeSyntax);
            var tuple = GetAttributeValueValue(rawItem);
            _value = tuple.Item1;
            _literalType = tuple.Item2;
        }
        internal RDomAttributeValue(
             RDomAttributeValue oldRDom)
            : base(oldRDom)
        { }

        private Tuple<object, LiteralType> GetAttributeValueValue(
                    AttributeArgumentSyntax arg)
        {
            // TODO: Manage multiple values because of AllowMultiples, param array, or missing symbol 
            var expr = arg.Expression;
            var literalType = LiteralType.Unknown;
            object value = null;
            var literalExpression = expr as LiteralExpressionSyntax;
            if (literalExpression != null)
            {
                value = GetLiteralValue(literalExpression, ref literalType);
            }
            else
            {
                var typeExpression = expr as TypeOfExpressionSyntax;
                if (typeExpression != null)
                {
                    literalType = LiteralType.Type;
                    value = GetTypeExpressionValue(typeExpression);
                }
            }
            return new Tuple<object, LiteralType>(value, literalType);
        }

        private object GetTypeExpressionValue(TypeOfExpressionSyntax typeExpression)
        {
            object value = null;
            var typeSyntax = typeExpression.Type;
            var predefinedTypeSyntax = typeSyntax as PredefinedTypeSyntax;
            var identifierNameSyntax = typeSyntax as IdentifierNameSyntax;
            if (predefinedTypeSyntax != null)
            {
                var typeInfo = GetTypeInfo(predefinedTypeSyntax);
                value = new RDomReferencedType(typeInfo, null);
            }
            else if (identifierNameSyntax != null)
            {
                var typeInfo = GetTypeInfo(identifierNameSyntax);
                value = new RDomReferencedType(typeInfo, null);
            }
            else
            {
                // I don't know how to get here, but if I get here, I want to know it :)
                throw new NotImplementedException();
            }
            return value;
        }

        private object GetLiteralValue(LiteralExpressionSyntax literalExpression, ref LiteralType literalType)
        {
            switch (literalExpression.Token.CSharpKind())
            {
                case SyntaxKind.StringLiteralToken:
                    literalType = LiteralType.String;
                    break;
                case SyntaxKind.NumericLiteralToken:
                    literalType = LiteralType.Numeric;
                    break;
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    literalType = LiteralType.Boolean;
                    break;
                default:
                    // I don't know how to get here, but if I get here, I want to know it :)
                    throw new NotImplementedException();
            }
            return literalExpression.Token.Value;
        }

        public override bool SameIntent(IAttributeValue other, bool includePublicAnnotations)
        {
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            var rDomOther = other as RDomAttributeValue;
            if (rDomOther == null) throw new InvalidOperationException();
            if (_name != rDomOther._name) return false;
            if (_literalType != rDomOther._literalType) return false;
            if (!(_value.Equals(rDomOther._value))) return false;
            return true;
        }

        private static string GetAttributeValueName(
            AttributeArgumentSyntax arg, AttributeSyntax attributeSyntax)
        {
            if (arg.NameColon != null)
            {
                return arg.NameColon.Name.ToString().Replace(":", "").Trim();
            }
            else if (arg.NameEquals != null)
            {
                return arg.NameEquals.Name.ToString();
            }
            else
            {
                return attributeSyntax.Name.ToString();
            }
            // TODO: Handle other naming scenarios
        }

        public override string Name
        {
            get { return _name; }
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        public LiteralType ValueType
        {
            get
            {
                return _literalType;
            }
        }

        internal static IEnumerable<IAttributeValue> MakeAttributeValues(
                AttributeSyntax attrib, RDomBase attributedItem)
        {
            var ret = new List<IAttributeValue>();
            if (attrib.ArgumentList != null)
            {
                var arguments = attrib.ArgumentList.Arguments;
                foreach (AttributeArgumentSyntax arg in arguments)
                {
                    ret.Add(new RDomAttributeValue(arg, attrib, attributedItem));
                }
            }
            return ret;
        }
    }
}
