using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoslynDom.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

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
            if (Symbol != null)
            { AccessModifier = (AccessModifier)Symbol.DeclaredAccessibility; }
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
