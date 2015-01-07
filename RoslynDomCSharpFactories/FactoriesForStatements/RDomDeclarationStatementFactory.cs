using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
   public class RDomDeclarationStatementFactory
       : RDomBaseSyntaxNodeFactory<RDomDeclarationStatement, VariableDeclaratorSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomDeclarationStatementFactory(RDomCorporation corporation)
         : base(corporation)
      { }

      private WhitespaceKindLookup WhitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.Constant, SyntaxKind.ConstKeyword);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      public override Type[] SupportedSyntaxNodeTypes
      { get { return new Type[] { typeof(LocalDeclarationStatementSyntax) }; } }

      protected override IEnumerable<IDom> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var list = new List<IStatementAndDetail>();
         var rawDeclaration = syntaxNode as LocalDeclarationStatementSyntax;

         // VariableDeclarationFactory does most of the work, and at present returns a single
         // DeclarationStatement, and possibly a comment
         var newItems = OutputContext.Corporation.Create(rawDeclaration.Declaration, parent, model, true);
         foreach (var newItem in newItems.OfType<IDeclarationStatement>())
         {
            CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);
            newItem.IsConst = rawDeclaration.IsConst;
         }
         list.AddRange(newItems.OfType<IStatementAndDetail>());
         return list;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IDeclarationStatement;
         var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);
         var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(itemAsT.IsImplicitlyTyped, itemAsT.Type);

         var variable = item as IVariableDeclaration;
         // This is a weakness in the current factory lookup - we can't just ask for a random factory
         // so to call the normal build syntax through the factory causes infinite recursion.
         // TODO: Add the ability to request a random factory from the container (via the CSharp uber factory
         var tempFactory = new RDomVariableDeclarationFactory(OutputContext.Corporation);
         var nodeDeclarators = tempFactory.BuildSyntax(item);
         var nodeDeclarator = (VariableDeclaratorSyntax)nodeDeclarators.First();

         var nodeDeclaratorInList = SyntaxFactory.SeparatedList(
                             SyntaxFactory.List<VariableDeclaratorSyntax>(
                                 new VariableDeclaratorSyntax[] { nodeDeclarator }));
         var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
         nodeDeclaration = BuildSyntaxHelpers.AttachWhitespace(nodeDeclaration, itemAsT.Whitespace2Set, WhitespaceLookup);
         var node = SyntaxFactory.LocalDeclarationStatement(nodeDeclaration);

         if (itemAsT.IsConst) { node = node.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ConstKeyword))); }

         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }
   }
}