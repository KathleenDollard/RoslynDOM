using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAttribute : RDomBase<IAttribute, AttributeSyntax, ISymbol>, IAttribute
    {
        private IEnumerable<IAttributeValue> _attributeValues;

        internal RDomAttribute(
            AttributeSyntax rawItem,
            IEnumerable<IAttributeValue> attributeValues)
            : base(rawItem)
        {
            _attributeValues = attributeValues;
            Initialize();
        }

        internal RDomAttribute(
            RDomAttribute oldRDom)
            : base(oldRDom)
        {
            var newAttributeValues = RDomBase<IAttributeValue>
                        .CopyMembers(oldRDom._attributeValues.Cast<RDomAttributeValue>());
            _attributeValues = newAttributeValues;
        }

        protected override void Initialize()
        {
            Name = TypedSyntax.Name.ToString();
        }

        public override bool SameIntent(IAttribute other, bool includePublicAnnotations)
        {
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            var rDomOther = other as RDomAttribute;
            if (rDomOther == null) throw new InvalidOperationException();
            return CheckSameIntentChildList(_attributeValues, rDomOther._attributeValues);
        }


        internal static IEnumerable<IAttribute> MakeAttributes(
            RDomBase attributedItem, ISymbol symbol, SyntaxNode syntax)
        {
            var ret = new List<IAttribute>();
            var symbolAttributes = symbol.GetAttributes();
            foreach (var attributeData in symbolAttributes)
            {
                // TODO: In those cases where we do have a symbol reference to the attribute, try to use it
                var appRef = attributeData.ApplicationSyntaxReference;
                var attribSyntax = syntax.FindNode(appRef.Span) as AttributeSyntax;
                var values = RDomAttributeValue.MakeAttributeValues(attribSyntax, attributedItem);
                var newAttribute = new RDomAttribute(attribSyntax, values);
                ret.Add(newAttribute);
            }
            return ret;
        }

        public IEnumerable<IAttributeValue> AttributeValues
        {
            get
            { return _attributeValues; }
        }
        
    }
}
