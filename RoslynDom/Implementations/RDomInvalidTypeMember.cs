using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    //public class RDomInvalidTypeMemberFactory
    //       : RDomTypeMemberFactory<RDomInvalidTypeMember, SyntaxNode>
    //{
    //    public override FactoryPriority Priority
    //    { get { return FactoryPriority.Normal - 1; } }
    //}

    public class RDomInvalidTypeMember : RDomBase<IInvalidTypeMember,  ISymbol>, IInvalidTypeMember
    {
        internal RDomInvalidTypeMember(
                     SyntaxNode rawItem)
           : base(rawItem)
        {
            Initialize();
        }

        internal RDomInvalidTypeMember(RDomInvalidTypeMember oldRDom)
             : base(oldRDom)
        {
            AccessModifier = oldRDom.AccessModifier;
        }
        protected override void Initialize()
        {
            base.Initialize();
            AccessModifier = GetAccessibility();
        }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier { get; set; }

        public MemberKind MemberKind
        {
            get { return MemberKind.InvalidMember; }
        }
    }
}
