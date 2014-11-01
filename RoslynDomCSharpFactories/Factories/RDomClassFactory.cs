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
   public class RDomClassTypeMemberFactory
    : RDomBaseSyntaxNodeFactory<RDomClass, ClassDeclarationSyntax>
   {
      public RDomClassTypeMemberFactory(RDomCorporation corporation)
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

      protected override IEnumerable<IDom> CreateListFromInterim(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ClassDeclarationSyntax;
         var newItem = new RDomClass(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
         CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, whitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.TypeParameterList, LanguagePart.Current, whitespaceLookup);

         newItem.Name = newItem.TypedSymbol.Name;

         newItem.MembersAll.CreateAndAdd(syntax, x => x.Members, x => OutputContext.Corporation.Create(x, newItem, model).Cast<ITypeMemberAndDetail>());

         newItem.IsAbstract = newItem.Symbol.IsAbstract;
         newItem.IsSealed = newItem.Symbol.IsSealed;
         newItem.IsStatic = newItem.Symbol.IsStatic;
         newItem.IsPartial = syntax.Modifiers.Contains(SyntaxFactory.Token(SyntaxKind.PartialKeyword ));

         return new IDom[] { newItem };
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IClass;
         Guardian.Assert.IsNotNull(itemAsT, nameof(itemAsT));

         var modifiers = item.BuildModfierSyntax();
         if (itemAsT.IsAbstract) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.AbstractKeyword)); }
         if (itemAsT.IsSealed) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.SealedKeyword)); }
         if (itemAsT.IsPartial) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword )); }
         var identifier = SyntaxFactory.Identifier(itemAsT.Name);

         var node = SyntaxFactory.ClassDeclaration(identifier)
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
