using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAttribute : RDomBase<IAttribute, ISymbol>, IAttribute
    {
        private List<IAttributeValue> _attributeValues = new List<IAttributeValue>();

        public RDomAttribute(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomAttribute(RDomAttribute oldRDom)
            : base(oldRDom)
        {
            var newAttributeValues = RoslynDomUtilities.CopyMembers(oldRDom._attributeValues);
            foreach (var value in newAttributeValues)
            { AddOrMoveAttributeValue(value); }
        }

        public string Name { get; set; }

        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }

        public void RemoveAttributeValue(IAttributeValue attributeValue)
        { _attributeValues.Remove(attributeValue); }

        public void AddOrMoveAttributeValue(IAttributeValue attributeValue)
        { _attributeValues.Add(attributeValue); }

        public IEnumerable<IAttributeValue> AttributeValues
        { get { return _attributeValues; } }

    }
}
