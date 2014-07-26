using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomEnumValue : RDomBase<IEnumValue, ISymbol>, IEnumValue
    {
        private AttributeList _attributes = new AttributeList();

        public RDomEnumValue(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomEnumValue(RDomEnumValue oldRDom)
             : base(oldRDom)
        {
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            if (oldRDom.Expression != null)
            { Expression = oldRDom.Expression.Copy(); }
            Name = oldRDom.Name;
        }

        public AttributeList Attributes
        { get { return _attributes; } }

        public string Name { get; set; }

        public string OuterName
        { get { return RoslynUtilities.GetOuterName(this); } }


        public IExpression Expression { get; set; }

        public string Description { get; set; }
    }
}
