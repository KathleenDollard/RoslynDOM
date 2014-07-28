using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{

    public class RDomNamespaceStemMemberFactory
           : RDomStemMemberFactory<RDomNamespace, NamespaceDeclarationSyntax>
    {
        public RDomNamespaceStemMemberFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as NamespaceDeclarationSyntax;
            // TODO: I think there is a better way to do this, but I can't find it right now
            var names = syntax.Name.ToString().Split(new char[] { '.' });
            var group = Guid.Empty;
            if (names.Count() > 1) group = Guid.NewGuid();
            RDomNamespace item = null;
            RDomNamespace outerNamespace = null;
            foreach (var name in names)
            {
                var newItem = new RDomNamespace(syntaxNode, parent, model, name, group);
                CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

                // At this point, item is the last newItem
                if (item != null) item.StemMembersAll.AddOrMove(newItem);
                item = newItem;
                if (outerNamespace == null) { outerNamespace = item; }
                if (name != names.Last()) { parent = item; }
            }

            // Qualified name unbundles namespaces, and if it's defined together, we want it together here. 
            // Thus, this replaces hte base Initialize name with the correct one
            if (item.Name.StartsWith("@")) { item.Name = item.Name.Substring(1); }
            CreateFromWorker.LoadStemMembers(item, syntax.Members, syntax.Usings, model);

            // This will return the outer namespace, which in the form N is the only one. 
            // In the form N1.N2.. there is a nested level for each part (N1, N2).
            // The inner holds the children, the outer is returned.  
            return outerNamespace;
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsNamespace = item as INamespace;
            var identifier = SyntaxFactory.IdentifierName(itemAsNamespace.Name);
            var node = SyntaxFactory.NamespaceDeclaration(identifier);
            if (itemAsNamespace == null) { throw new InvalidOperationException(); }
            var usingsSyntax = itemAsNamespace.UsingDirectives
                        .Select(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                        .OfType<UsingDirectiveSyntax>()
                        .ToList();
            if (usingsSyntax.Count() > 0) { node = node.WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>(usingsSyntax)); }

            var membersSyntax = itemAsNamespace.StemMembers
                        .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                        .OfType<MemberDeclarationSyntax>()
                        .ToList();
            if (membersSyntax.Count() > 0) { node = node.WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(membersSyntax)); }

            return item.PrepareForBuildSyntaxOutput(node);
        }
    }


}
