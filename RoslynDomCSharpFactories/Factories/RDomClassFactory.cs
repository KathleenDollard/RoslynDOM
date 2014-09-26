using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   internal static class RDomClassFactoryHelper
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
               _whitespaceLookup.Add(LanguageElement.ClassKeyword, SyntaxKind.ClassKeyword);
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookup.Add(LanguageElement.Sealed, SyntaxKind.SealedKeyword);
               _whitespaceLookup.Add(LanguageElement.Abstract, SyntaxKind.AbstractKeyword);
               _whitespaceLookup.Add(LanguageElement.ClassStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.ClassEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.Add(LanguageElement.TypeParameterStartDelimiter, SyntaxKind.LessThanToken);
               _whitespaceLookup.Add(LanguageElement.TypeParameterEndDelimiter, SyntaxKind.GreaterThanToken);
               _whitespaceLookup.Add(LanguageElement.BaseListPrefix, SyntaxKind.ColonToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.StaticModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      internal static RDomClass CreateFromInternal(SyntaxNode syntaxNode, IDom parent, SemanticModel model,
             ICSharpCreateFromWorker createFromWorker, RDomCorporation corporation)
      {
         var syntax = syntaxNode as ClassDeclarationSyntax;
         var newItem = new RDomClass(syntaxNode, parent, model);
         createFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
         createFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, whitespaceLookup);
         createFromWorker.StoreWhitespace(newItem, syntax.TypeParameterList, LanguagePart.Current, whitespaceLookup);

         newItem.Name = newItem.TypedSymbol.Name;

         var members = ListUtilities.MakeList(syntax, x => x.Members, x => corporation.CreateFrom<ITypeMemberCommentWhite>(x, newItem, model));
         newItem.MembersAll.AddOrMoveRange(members);

         newItem.IsAbstract = newItem.Symbol.IsAbstract;
         newItem.IsSealed = newItem.Symbol.IsSealed;
         newItem.IsStatic = newItem.Symbol.IsStatic;

         return newItem;
      }

      public static IEnumerable<SyntaxNode> BuildSyntax(RDomClass item,
                   ICSharpBuildSyntaxWorker buildSyntaxWorker,
                   RDomCorporation corporation)
      {
         var itemAsT = item as IClass;
         Guardian.Assert.IsNotNull(itemAsT, nameof(itemAsT));

         var modifiers = item.BuildModfierSyntax();
         if (item.IsAbstract) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.AbstractKeyword)); }
         if (item.IsSealed) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.SealedKeyword)); }
         var identifier = SyntaxFactory.Identifier(item.Name);

         var node = SyntaxFactory.ClassDeclaration(identifier)
             .WithModifiers(modifiers);
         node = BuildSyntaxHelpers.AttachWhitespace(node, item.Whitespace2Set, whitespaceLookup);

         var baseList = BuildSyntaxHelpers.GetBaseList(item);
         if (baseList != null) { node = node.WithBaseList(baseList); }

         var attributes = buildSyntaxWorker.BuildAttributeSyntax(item.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         var membersSyntax = itemAsT.Members
                     .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                     .ToList();
         node = node.WithMembers(SyntaxFactory.List(membersSyntax));

         node = BuildSyntaxHelpers.BuildTypeParameterSyntax(
                    itemAsT, node, whitespaceLookup,
                    (x, p) => x.WithTypeParameterList(p),
                    (x, c) => x.WithConstraintClauses(c));

         return node.PrepareForBuildSyntaxOutput(item);
      }

   }


   public class RDomClassTypeMemberFactory
          : RDomTypeMemberFactory<RDomClass, ClassDeclarationSyntax>
   {
      public RDomClassTypeMemberFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         return RDomClassFactoryHelper.CreateFromInternal(syntaxNode, parent, model, CreateFromWorker, Corporation);
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         return RDomClassFactoryHelper.BuildSyntax((RDomClass)item, BuildSyntaxWorker, Corporation);
      }
   }

   public class RDomClassStemMemberFactory
          : RDomStemMemberFactory<RDomClass, ClassDeclarationSyntax>
   {
      public RDomClassStemMemberFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         return RDomClassFactoryHelper.CreateFromInternal(syntaxNode, parent, model, CreateFromWorker, Corporation);
      }
      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         return RDomClassFactoryHelper.BuildSyntax((RDomClass)item, BuildSyntaxWorker, Corporation);
      }
   }

}
