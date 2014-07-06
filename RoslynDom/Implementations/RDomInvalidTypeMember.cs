using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomInvalidTypeMember : RDomBase<IInvalidTypeMember, SyntaxNode, ISymbol>, IInvalidTypeMember
    {
        internal RDomInvalidTypeMember(
                     SyntaxNode rawItem,
                     params PublicAnnotation[] publicAnnotations)
            : base(rawItem, publicAnnotations )
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

        protected override bool CheckSameIntent(IInvalidTypeMember other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            if (!base.CheckSameIntent(other, includePublicAnnotations)) return false;
            if (AccessModifier != other.AccessModifier) return false;
            return true;
        }
        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier { get; set; }

        public MemberType MemberType
        {
            get { return MemberType.InvalidMember; }
        }
    }
}
