using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{

    public class RDomNamespaceStemMemberFactory
           : RDomStemMemberFactory<RDomNamespace, NamespaceDeclarationSyntax>
    { }


    public class RDomNamespace : RDomBaseStemContainer<INamespace, NamespaceDeclarationSyntax, INamespaceSymbol>, INamespace
    {
        //private string _outerName;

        internal RDomNamespace(NamespaceDeclarationSyntax rawItem)
           : base(rawItem)
        {
            Initialize2();
        }

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

        protected void Initialize2()
        {
            Initialize();
            var members = ListUtilities.MakeList(TypedSyntax, x => x.Members, x => RDomFactoryHelper.StemMemberFactoryHelper.MakeItem(x));
            var usings = ListUtilities.MakeList(TypedSyntax, x => x.Usings, x => RDomFactoryHelper.StemMemberFactoryHelper.MakeItem(x));
            foreach (var member in members)
            { AddOrMoveStemMember(member); }
            foreach (var member in usings)
            { AddOrMoveStemMember(member); }
        }

        public override NamespaceDeclarationSyntax BuildSyntax()
        {
            var node = SyntaxFactory.NamespaceDeclaration (SyntaxFactory.IdentifierName(Name));

            node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildUsings(), node, (n, l) => n.WithUsings(l));
            node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildStemMembers(), node, (n, l) => n.WithMembers(l));

            return (NamespaceDeclarationSyntax)RoslynUtilities.Format(node);
        }

        public override string OuterName
        { get { return QualifiedName; } }

        public StemMemberKind StemMemberKind
        { get { return StemMemberKind.Namespace; } }
    }
}
