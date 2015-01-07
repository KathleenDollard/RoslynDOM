using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
   public class RDomNamespaceStemMemberFactory
          : RDomBaseSyntaxNodeFactory<RDomNamespace, NamespaceDeclarationSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomNamespaceStemMemberFactory(RDomCorporation corporation)
         : base(corporation)
      { }

      private WhitespaceKindLookup WhitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.NamespaceKeyword, SyntaxKind.NamespaceKeyword);
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookup.Add(LanguageElement.NamespaceStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.NamespaceEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
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
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

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
         var itemAsT = item as INamespace;
         var innerItem = itemAsT;
         var groupGuid = itemAsT.GroupGuid;
         var name = itemAsT.Name;
         if (groupGuid != Guid.Empty)
         {
            var nestedNamespaces = itemAsT.Descendants
                                    .OfType<INamespace>()
                                    .Where(x => x.GroupGuid == groupGuid);
            foreach (var nested in nestedNamespaces)
            {
               name += "." + nested.Name;
               innerItem = nested;
            }
         }
         var identifier = SyntaxFactory.IdentifierName(name);
         NamespaceDeclarationSyntax node = BuidSyntax(innerItem, identifier);

         return node.PrepareForBuildSyntaxOutput(innerItem, OutputContext);
      }

      private NamespaceDeclarationSyntax BuidSyntax(INamespace itemAsNamespace, IdentifierNameSyntax identifier)
      {
         var node = SyntaxFactory.NamespaceDeclaration(identifier);
         Guardian.Assert.IsNotNull(itemAsNamespace, nameof(itemAsNamespace));
         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsNamespace.Whitespace2Set, WhitespaceLookup);

         var usingDirectives = itemAsNamespace.UsingDirectives
                                 .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x));
         var usingsSyntax = usingDirectives
                        .OfType<UsingDirectiveSyntax>()
                        .ToList();
         if (usingsSyntax.Count() > 0) { node = node.WithUsings(SyntaxFactory.List<UsingDirectiveSyntax>(usingsSyntax)); }

         var membersSyntax = itemAsNamespace.StemMembers
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .OfType<MemberDeclarationSyntax>()
                     .ToList();
         if (membersSyntax.Count() > 0) { node = node.WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(membersSyntax)); }

         return node;
      }
   }
}