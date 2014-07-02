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
        }

        internal RDomAttribute(
            RDomAttribute oldRDom)
            : base(oldRDom)
        {
            var newAttributeValues = RDomBase<IAttributeValue>
                        .CopyMembers(oldRDom._attributeValues.Cast<RDomAttributeValue>());
            _attributeValues = newAttributeValues;
        }

        public override bool SameIntent(IAttribute other, bool includePublicAnnotations)
        {
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            var rDomOther = other as RDomAttribute;
            if (rDomOther == null) throw new InvalidOperationException();
            return CheckSameIntentChildList(_attributeValues, rDomOther._attributeValues);
            //return CheckSameIntentChildList(_attributeValues,
            //            rDomOther._attributeValues,
            //            (x, y) => x.Name == y.Name);
            //if (_attributeValues == null) return (rDomOther._attributeValues == null);
            //if (rDomOther._attributeValues == null) return false;
            //if (_attributeValues.Count() != rDomOther._attributeValues.Count()) return false;
            //foreach (var attributeValue in _attributeValues)
            //{
            //    // TODO: Multiple attribute values aren't legal, but might still mess us up here
            //    var otherAttributeValue = rDomOther._attributeValues.Where(x => x.Name == attributeValue.Name).FirstOrDefault();
            //    if (otherAttributeValue == null) return false;
            //    if (!attributeValue.SameIntent(otherAttributeValue)) return false;
            //}
            //return true;
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

        public override string Name
        {
            get { return TypedSyntax.Name.ToString(); }
        }

    }
}
