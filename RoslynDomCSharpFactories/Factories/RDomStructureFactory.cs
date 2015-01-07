using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RoslynDom.CSharp
{
   public class RDomStructureFactory
     : RDomBaseSyntaxNodeFactory<RDomStructure, StructDeclarationSyntax>
   {
      public RDomStructureFactory(RDomCorporation corporation)
         : base(corporation)
      { }

      // until move to C# 6 - I want to support name of as soon as possible
      [ExcludeFromCodeCoverage]
      private static string nameof<T>(T value) { return ""; }

      private static WhitespaceKindLookup _whitespaceLookup;

      private static WhitespaceKindLookup whitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.StructureKeyword, SyntaxKind.StructKeyword);
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookup.Add(LanguageElement.StructureStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.StructureEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IEnumerable<IDom> CreateListFromInterim(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as StructDeclarationSyntax;
         var newItem = new RDomStructure(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, whitespaceLookup);

         newItem.MembersAll.CreateAndAdd(syntax, x => x.Members, x => OutputContext.Corporation.Create(x, newItem, model).Cast<ITypeMemberAndDetail>());

         return new IDom[] { newItem };
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IStructure;
         Guardian.Assert.IsNotNull(itemAsT, nameof(itemAsT));

         // This is identical to Class, but didn't work out reuse here
         var modifiers = item.BuildModfierSyntax();
         var identifier = SyntaxFactory.Identifier(itemAsT.Name);

         var node = SyntaxFactory.StructDeclaration(identifier)
             .WithModifiers(modifiers);
         node = BuildSyntaxHelpers.AttachWhitespace(node, item.Whitespace2Set, whitespaceLookup);

         var baseList = BuildSyntaxHelpers.GetBaseList(itemAsT);
         if (baseList != null) { node = node.WithBaseList(baseList); }

         var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         var membersSyntax = itemAsT.Members
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .ToList();
         node = node.WithMembers(SyntaxFactory.List(membersSyntax));

         node = BuildSyntaxHelpers.BuildTypeParameterSyntax(
                    itemAsT, node, whitespaceLookup,
                    (x, p) => x.WithTypeParameterList(p),
                    (x, c) => x.WithConstraintClauses(c));

         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }
   }
}