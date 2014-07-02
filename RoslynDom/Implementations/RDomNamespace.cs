using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{

    public class RDomNamespace : RDomBaseStemContainer<INamespace, NamespaceDeclarationSyntax, INamespaceSymbol>, INamespace
    {
        internal RDomNamespace(NamespaceDeclarationSyntax rawItem,
            IEnumerable<IStemMember> members,
            IEnumerable<IUsing> usings,
            params PublicAnnotation[] publicAnnotations)
            : base(rawItem, members, usings, publicAnnotations)
        { }

        public override bool SameIntent(INamespace other, bool includePublicAnnotations)
        {
            // Base class checks classes, etc
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            if (!CheckSameIntentChildList(NonEmptyNamespaces, other.NonEmptyNamespaces)) return false;
            return true;
        }

        public IEnumerable<INamespace> AllChildNamespaces
        {
            get { return RoslynDomUtilities.GetAllChildNamespaces(this); }
        }

         public IEnumerable<INamespace> NonEmptyNamespaces
        {
            get { return RoslynDomUtilities.GetNonEmptyNamespaces(this); }
        }


        public override string OuterName
        {
            get
            {
                var namespaceName = RoslynDomUtilities.GetContainingNamespaceName(Symbol.ContainingNamespace);
                namespaceName = string.IsNullOrWhiteSpace(namespaceName) ? "" : namespaceName + ".";
                return namespaceName + Name;
            }
        }

         public StemMemberType StemMemberType
        {
            get
            { return StemMemberType.Namespace; }
        }
    }
}
