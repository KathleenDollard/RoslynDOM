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

    public class RDomNamespace : RDomBaseStemContainer<NamespaceDeclarationSyntax, INamespaceSymbol>, INamespace
    {
        internal RDomNamespace(NamespaceDeclarationSyntax rawItem,
            IEnumerable<IStemMember> members,
            IEnumerable<IUsing> usings)
            : base(rawItem, members, usings)
        { }

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
