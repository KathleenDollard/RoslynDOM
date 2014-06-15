using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAttribute : RDomSyntaxNodeBase<AttributeSyntax, ISymbol>, IAttribute
    {
        private IEnumerable<IAttributeValue> _attributeValues;
        internal RDomAttribute(
            AttributeSyntax rawItem,
            IEnumerable<IAttributeValue> attributeValues)
            : base(rawItem)
        {
            _attributeValues = attributeValues;
        }

        public static IEnumerable<IAttribute > MakeAttributes(ISymbol symbol, SyntaxNode syntax)
        {
            var ret = new List<IAttribute>();
            var symbolAttributes = symbol.GetAttributes();
            foreach (var symbolAttribute in symbolAttributes )
            {
                // TODO: In those cases where we do have a symbol reference to the attribute, try to use it
                var appRef = symbolAttribute.ApplicationSyntaxReference;
                var attrib = syntax.FindNode(appRef.Span) as AttributeSyntax ;
                var name = attrib.Name;
                var values = RDomAttributeValue.MakeAttributeValues(attrib, symbol);
                var newAttribute = new RDomAttribute(attrib, values);
                ret.Add(newAttribute);
            }
            return ret;
        }

         public IEnumerable<IAttributeValue> AttributeValues
        {
            get
            { return _attributeValues; }
        }

        public override string Name
        {
            get { return TypedSyntax.Name.ToString(); }
        }

    }
}
