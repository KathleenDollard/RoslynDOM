using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{

    public class RDomNamespace : RDomBaseStemContainer<INamespace, NamespaceDeclarationSyntax, INamespaceSymbol>, INamespace
    {
        //private string _outerName;

        internal RDomNamespace(NamespaceDeclarationSyntax rawItem,
            IEnumerable<IStemMember> members,
            IEnumerable<IUsing> usings,
            params PublicAnnotation[] publicAnnotations)
            : base(rawItem, members, usings, publicAnnotations)
        {
            Initialize();
        }

        internal RDomNamespace(RDomNamespace oldRDom)
             : base(oldRDom)
        {
            //_outerName = oldRDom.OuterName;

        }

        protected override void Initialize()
        {
            base.Initialize();
            // Qualified name unbundles namespaces, and if it's defined together, we want it together here. 
            // Thus, this replaces hte base Initialize name with the correct one
            Name = TypedSyntax.NameFrom();
            if (Name.StartsWith("@")) { Name = Name.Substring(1); }
        }

        public IEnumerable<INamespace> AllChildNamespaces
        {
            get { return RoslynDomUtilities.GetAllChildNamespaces(this); }
        }

        public IEnumerable<INamespace> NonemptyNamespaces
        {
            get { return RoslynDomUtilities.GetNonEmptyNamespaces(this); }
        }

        public override string OuterName
        { get { return QualifiedName; } }

        public StemMemberKind StemMemberKind
        { get { return StemMemberKind.Namespace; } }
    }
}
