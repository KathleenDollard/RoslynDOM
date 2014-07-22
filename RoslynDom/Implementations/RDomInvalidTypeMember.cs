using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{
    public class RDomInvalidTypeMember : RDomBase<IInvalidTypeMember, ISymbol>, IInvalidTypeMember
    {
        private AttributeList _attributes = new AttributeList();

        public RDomInvalidTypeMember(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomInvalidTypeMember(RDomInvalidTypeMember oldRDom)
             : base(oldRDom)
        {
            Attributes.AddOrMoveAttributeRange(oldRDom.Attributes.Select(x => x.Copy()));
            AccessModifier = oldRDom.AccessModifier;
        }
        protected override void Initialize()
        {
            base.Initialize();
            AccessModifier = GetAccessibility();
        }

        public string Name { get; set; }

        public AttributeList Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }

        public MemberKind MemberKind
        {
            get { return MemberKind.InvalidMember; }
        }

        public IStructuredDocumentation StructuredDocumentation { get; set; }

        public string Description { get; set; }
    }
}
