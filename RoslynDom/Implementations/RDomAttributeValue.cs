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
    public class RDomAttributeValue : RDomSyntaxNodeBase<AttributeArgumentSyntax, ISymbol>, IAttributeValue
    {
        private string _name;
        private LiteralType _literalType;
        private object _value;

        internal RDomAttributeValue(
            AttributeArgumentSyntax rawItem,
            AttributeSyntax attributeSyntax,
            ISymbol attributeSymbol)
            : base(rawItem)
        {
            _name = GetAttributeValueName(rawItem, attributeSyntax, attributeSymbol);
            var tuple = GetAttributeValueValue(rawItem, attributeSyntax, attributeSymbol);
            _value = tuple.Item1;
            _literalType = tuple.Item2;
        }

        private Tuple<object, LiteralType> GetAttributeValueValue(AttributeArgumentSyntax arg, AttributeSyntax attributeSyntax, ISymbol attributeSymbol)
        {
            // TODO: Manage multiple values because of AllowMultiples, param array, or missing symbol 
            var expr = arg.Expression;
            var literalExpression = expr as LiteralExpressionSyntax;
            object value = null;
           var literalType = LiteralType.Unknown ;
            if (literalExpression != null)
            {
                switch (literalExpression.Token.CSharpKind())
                {
                    case SyntaxKind.StringLiteralToken:
                        literalType = LiteralType.String;
                        break;
                    case SyntaxKind.NumericLiteralToken:
                        literalType = LiteralType.Numeric ;
                        break;
                    case SyntaxKind.TrueKeyword:
                    case SyntaxKind.FalseKeyword:
                        literalType = LiteralType.Boolean;
                        break;
                    default:
                        break;
                }
                value = literalExpression.Token.Value;
            }
            var typeExpression = expr as TypeOfExpressionSyntax;
            if (typeExpression != null)
            {
                var typeSyntax = typeExpression.Type;
                literalType = LiteralType.Type;
                throw new NotImplementedException();
            }
            return new Tuple<object, LiteralType >(value, literalType);
        }

        private string GetAttributeValueName(AttributeArgumentSyntax arg, AttributeSyntax attributeSyntax, ISymbol attributeSymbol)
        {
            if (arg.NameColon != null)
            {
                throw new NotImplementedException();
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
                return _literalType ;
            }
        }

        internal static IEnumerable<IAttributeValue> MakeAttributeValues(AttributeSyntax attrib, ISymbol symbol)
        {
            var ret = new List<IAttributeValue>();
            if (attrib.ArgumentList != null)
            {
                var arguments = attrib.ArgumentList.Arguments;
                foreach (AttributeArgumentSyntax arg in arguments)
                {
                    ret.Add(new RDomAttributeValue(arg, attrib, symbol));
                }
            }
            return ret;
        }
    }
}
