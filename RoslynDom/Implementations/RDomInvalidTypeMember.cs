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
    public class RDomInvalidTypeMember : RDomSyntaxNodeBase<IInvalidTypeMember, SyntaxNode, ISymbol>, IInvalidTypeMember
    {
        internal RDomInvalidTypeMember(
                     SyntaxNode rawItem,
                     params PublicAnnotation[] publicAnnotations)
            : base(rawItem, publicAnnotations )
        { }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier
        {
            get
            {
                Accessibility accessibility = Symbol.DeclaredAccessibility;
                return (AccessModifier)accessibility;
            }
        }

        public MemberType MemberType
        {
            get { return MemberType.InvalidMember; }
        }
    }
}
