using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomEnumMember : RDomBase<IEnumMember, ISymbol>, IEnumMember
    {
        private AttributeList _attributes = new AttributeList();

        public RDomEnumMember(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomEnumMember(RDomEnumMember oldRDom)
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
