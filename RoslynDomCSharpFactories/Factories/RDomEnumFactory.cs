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
   internal static class RDomEnumFactoryHelper
   {
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
               _whitespaceLookup.Add(LanguageElement.EnumKeyword, SyntaxKind.EnumKeyword);
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookup.Add(LanguageElement.EnumValuesStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.EnumValuesEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      public static RDomEnum CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model,
          ICSharpCreateFromWorker createFromWorker, RDomCorporation corporation)
      {
         var syntax = syntaxNode as EnumDeclarationSyntax;
         var newItem = new RDomEnum(syntaxNode, parent, model);
         createFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
         createFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, whitespaceLookup);

         InitializeBaseList(syntax, newItem, model, createFromWorker, corporation);
         newItem.Name = newItem.TypedSymbol.Name;

         var members = ListUtilities.MakeList(syntax, x => x.Members, x => corporation.Create(x, newItem, model))
                         .OfType<IEnumMember>();
         newItem.Members.AddOrMoveRange(members);

         return newItem;
      }

      private static void InitializeBaseList(EnumDeclarationSyntax syntax, RDomEnum newItem, SemanticModel model,
          ICSharpCreateFromWorker createFromWorker, RDomCorporation corporation)
      {
         var symbol = newItem.Symbol as INamedTypeSymbol;
         if (symbol != null)
         {
            var underlyingNamedTypeSymbol = symbol.EnumUnderlyingType;
            // TODO: underlying type should be set to Int when there is not type specified,there is always an underlying type and the int default might be language specific
            if (syntax.BaseList != null)
            {
               createFromWorker.StoreWhitespaceForToken(newItem, syntax.BaseList.ColonToken,
                             LanguagePart.Current, LanguageElement.BaseListPrefix);
               var type = corporation
                               .Create(syntax.BaseList.Types.First(), newItem, model)
                               .FirstOrDefault()
                               as IReferencedType;
               newItem.UnderlyingType = type;
               createFromWorker.StoreWhitespace(type, syntax.BaseList,
                             LanguagePart.Current, whitespaceLookup);
            }
         }
      }


      public static IEnumerable<SyntaxNode> BuildSyntax(RDomEnum item, ICSharpBuildSyntaxWorker buildSyntaxWorker, RDomCorporation corporation)
      {
         var itemAsT = item as IEnum;
         Guardian.Assert.IsNotNull(itemAsT, nameof(itemAsT));

         var modifiers = item.BuildModfierSyntax();
         var identifier = SyntaxFactory.Identifier(item.Name);
         var node = SyntaxFactory.EnumDeclaration(identifier)
             .WithModifiers(modifiers);

         var baseList = GetBaseList(itemAsT);
         if (baseList != null) { node = node.WithBaseList(baseList); }

         var attributes = buildSyntaxWorker.BuildAttributeSyntax(item.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         var memberList = itemAsT.Members
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .OfType<EnumMemberDeclarationSyntax>()
                     .ToList();
         if (memberList.Any())
         {
            var memberListSyntax = SyntaxFactory.SeparatedList(memberList);
            node = node.WithMembers(memberListSyntax);
         }

         node = BuildSyntaxHelpers.AttachWhitespace(node, item.Whitespace2Set, whitespaceLookup);
         return node.PrepareForBuildSyntaxOutput(item);
      }

      public static BaseListSyntax GetBaseList(IEnum item)
      {
         if (item.UnderlyingType != null)
         {
            var underlyingTypeSyntax = (TypeSyntax)RDom.CSharp.GetSyntaxNode(item.UnderlyingType);
            underlyingTypeSyntax = BuildSyntaxHelpers.AttachWhitespace(
                underlyingTypeSyntax, item.UnderlyingType.Whitespace2Set, whitespaceLookup);

            var colonToken = SyntaxFactory.Token(SyntaxKind.ColonToken);
            colonToken = BuildSyntaxHelpers.AttachWhitespaceToToken(colonToken, item.Whitespace2Set[LanguageElement.BaseListPrefix]);
            return SyntaxFactory.BaseList(colonToken,
                SyntaxFactory.SingletonSeparatedList(underlyingTypeSyntax));
         }
         return null;
      }
   }

   public class RDomEnumTypeMemberFactory
        : RDomTypeMemberFactory<RDomEnum, EnumDeclarationSyntax>
   {
      public RDomEnumTypeMemberFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      protected override IEnumerable<IDom> CreateListFromInterim(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var ret = RDomEnumFactoryHelper.CreateFrom(syntaxNode, parent, model, CreateFromWorker, Corporation);
         return new IDom[] { ret };
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         return RDomEnumFactoryHelper.BuildSyntax((RDomEnum)item, BuildSyntaxWorker, Corporation);
      }
   }

   //public class RDomEnumTypeMemberFactory
   //    : RDomTypeMemberFactory<RDomEnum, EnumDeclarationSyntax>
   //{
   //   public RDomEnumTypeMemberFactory(RDomCorporation corporation)
   //      : base(corporation)
   //   { }

   //   protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
   //   {
   //      var ret = RDomEnumFactoryHelper.CreateFrom(syntaxNode, parent, model, CreateFromWorker, Corporation);
   //      return ret;
   //   }

   //   public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
   //   {
   //      return RDomEnumFactoryHelper.BuildSyntax((RDomEnum)item, BuildSyntaxWorker, Corporation);
   //   }
   //}


   //public class RDomEnumStemMemberFactory
   //       : RDomStemMemberFactory<RDomEnum, EnumDeclarationSyntax>
   //{
   //   public RDomEnumStemMemberFactory(RDomCorporation corporation)
   //      : base(corporation)
   //   { }

   //   protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
   //   {
   //      return RDomEnumFactoryHelper.CreateFrom(syntaxNode, parent, model, CreateFromWorker, Corporation);
   //   }

   //   public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
   //   {
   //      return RDomEnumFactoryHelper.BuildSyntax((RDomEnum)item, BuildSyntaxWorker, Corporation);
   //   }
   //}

}
