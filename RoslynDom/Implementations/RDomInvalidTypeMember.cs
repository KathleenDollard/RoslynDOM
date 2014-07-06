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

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier { get; set; }

        public MemberKind MemberKind
        {
            get { return MemberKind.InvalidMember; }
        }
    }
}
