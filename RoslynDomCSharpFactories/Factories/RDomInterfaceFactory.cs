using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace RoslynDom.CSharp
{
   internal static class RDomInterfaceFactoryHelper
   {
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
               _whitespaceLookup.Add(LanguageElement.InterfaceKeyword, SyntaxKind.InterfaceKeyword);
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookup.Add(LanguageElement.InterfaceStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.InterfaceEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.Add(LanguageElement.TypeParameterStartDelimiter, SyntaxKind.LessThanToken);
               _whitespaceLookup.Add(LanguageElement.TypeParameterEndDelimiter, SyntaxKind.GreaterThanToken);
               _whitespaceLookup.Add(LanguageElement.BaseListPrefix, SyntaxKind.ColonToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      public static RDomInterface CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model, ICSharpCreateFromWorker createFromWorker, RDomCorporation corporation)
      {
         var syntax = syntaxNode as InterfaceDeclarationSyntax;
         var newItem = new RDomInterface(syntaxNode, parent, model);
         createFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
         createFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, whitespaceLookup);
         createFromWorker.StoreWhitespace(newItem, syntax.TypeParameterList, LanguagePart.Current, whitespaceLookup);

         newItem.Name = newItem.TypedSymbol.Name;

         newItem.MembersAll.CreateAndAdd(syntax, x => x.Members, x => corporation.Create(x, newItem, model).Cast<ITypeMemberAndDetail>());
         // this is a hack because the membersare appearing with a scope
         foreach (var member in newItem.MembersAll.OfType<ITypeMember>())
         { member.AccessModifier = AccessModifier.None; }

         return newItem;
      }

      public static IEnumerable<SyntaxNode> BuildSyntax(IDom item, ICSharpBuildSyntaxWorker buildSyntaxWorker, RDomCorporation corporation)
      {
         var itemAsT = item as IInterface;
         Guardian.Assert.IsNotNull(itemAsT, nameof(itemAsT));

         var modifiers = itemAsT.BuildModfierSyntax();
         var identifier = SyntaxFactory.Identifier(itemAsT.Name);

         var node = SyntaxFactory.InterfaceDeclaration(identifier)
             .WithModifiers(modifiers);

         var baseList = BuildSyntaxHelpers.GetBaseList(itemAsT);
         if (baseList != null) { node = node.WithBaseList(baseList); }

         var attributes = buildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         var membersSyntax = itemAsT.Members
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .ToList();
         node = node.WithMembers(SyntaxFactory.List(membersSyntax));

         node = BuildSyntaxHelpers.BuildTypeParameterSyntax(
                   itemAsT, node, whitespaceLookup,
                   (x, p) => x.WithTypeParameterList(p),
                   (x, c) => x.WithConstraintClauses(c));

         //// This works oddly because it uncollapses the list
         //// This code is largely repeated in class and method factories, but is very hard to refactor because of shallow Roslyn (Microsoft) architecture
         //var typeParamsAndConstraints = itemAsT.TypeParameters
         //            .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
         //            .ToList();

         //var typeParameterSyntaxList = BuildSyntaxHelpers.GetTypeParameterSyntaxList(
         //            typeParamsAndConstraints, itemAsT.Whitespace2Set, whitespaceLookup);
         //if (typeParameterSyntaxList != null)
         //{
         //    node = node.WithTypeParameterList(typeParameterSyntaxList);
         //    var clauses = BuildSyntaxHelpers.GetTypeParameterConstraintList(
         //              typeParamsAndConstraints, itemAsT.Whitespace2Set, whitespaceLookup);
         //    if (clauses.Any())
         //    { node = node.WithConstraintClauses(clauses); }
         //}

         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, whitespaceLookup);
         return node.PrepareForBuildSyntaxOutput(item);
      }
   }

   public class RDomInterfaceTypeMemberFactory
      : RDomBaseItemFactory<RDomInterface, InterfaceDeclarationSyntax>
   {
      public RDomInterfaceTypeMemberFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      protected override IEnumerable<IDom> CreateListFromInterim(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var ret = RDomInterfaceFactoryHelper.CreateFrom(syntaxNode, parent, model, CreateFromWorker, Corporation);
         return new IDom[] { ret };
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item, BuildSyntaxWorker, Corporation);
      }
   }

   //public class RDomInterfaceTypeMemberFactory
   //   : RDomBaseItemFactory<RDomInterface, InterfaceDeclarationSyntax>
   //{
   //   public RDomInterfaceTypeMemberFactory(RDomCorporation corporation)
   //      : base(corporation)
   //   { }

   //   protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
   //   {
   //      return RDomInterfaceFactoryHelper.CreateFrom(syntaxNode, parent, model, CreateFromWorker, Corporation);
   //   }
   //   public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
   //   {
   //      return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item, BuildSyntaxWorker, Corporation);
   //   }
   //}


   //public class RDomInterfaceStemMemberFactory
   //       : RDomBaseItemFactory<RDomInterface, InterfaceDeclarationSyntax>
   //{
   //   public RDomInterfaceStemMemberFactory(RDomCorporation corporation)
   //      : base(corporation)
   //   { }

   //   protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
   //   {
   //      return RDomInterfaceFactoryHelper.CreateFrom(syntaxNode, parent, model, CreateFromWorker, Corporation);
   //   }
   //   public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
   //   {
   //      return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item, BuildSyntaxWorker, Corporation);
   //   }
   //}



}
