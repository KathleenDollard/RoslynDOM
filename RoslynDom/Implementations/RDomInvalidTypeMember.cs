using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{
    //public class RDomInvalidTypeMemberFactory
    //       : RDomTypeMemberFactory<RDomInvalidTypeMember, SyntaxNode>
    //{
    //    public override FactoryPriority Priority
    //    { get { return FactoryPriority.Normal - 1; } }
    //}

    public class RDomInvalidTypeMember : RDomBase<IInvalidTypeMember, ISymbol>, IInvalidTypeMember
    {
        private AttributeList  _attributes = new AttributeList();

        public RDomInvalidTypeMember(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomInvalidTypeMember(RDomInvalidTypeMember oldRDom)
             : base(oldRDom)
        {
                        Attributes.AddOrMoveAttributeRange( oldRDom.Attributes.Select(x=>x.Copy()));
            AccessModifier = oldRDom.AccessModifier;
        }
        protected override void Initialize()
        {
            base.Initialize();
            AccessModifier = GetAccessibility();
        }

         public AttributeList Attributes
        { get { return _attributes; } }

        public AccessModifier AccessModifier { get; set; }

        public MemberKind MemberKind
        {
            get { return MemberKind.InvalidMember; }
        }
    }
}
