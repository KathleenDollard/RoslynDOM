using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class RDomUsingDirectiveStemMemberFactory
           : RDomBaseItemFactory<RDomUsingDirective, UsingDirectiveSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomUsingDirectiveStemMemberFactory(RDomCorporation corporation)
       : base(corporation)
      { }

      private WhitespaceKindLookup WhitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.UsingKeyword, SyntaxKind.UsingKeyword);
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }



      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as UsingDirectiveSyntax;
         var newItem = new RDomUsingDirective(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

         newItem.Name = syntax.Name.NameFrom();
         if (syntax.Alias != null)
         { newItem.Alias = syntax.Alias.ToString().Replace("=", "").Trim(); }

         return newItem;
      }


      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         // TODO: Handle alias's
         // TODO: Handle using statements, that's not done (the other usings)
         var itemAsT = item as IUsingDirective;
         var identifier = SyntaxFactory.IdentifierName(itemAsT.Name);
         var node = SyntaxFactory.UsingDirective(identifier);
         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

         if (!string.IsNullOrWhiteSpace(itemAsT.Alias))
         { node = node.WithAlias(SyntaxFactory.NameEquals(itemAsT.Alias)); }
         return node.PrepareForBuildSyntaxOutput(item);
      }
   }

}
