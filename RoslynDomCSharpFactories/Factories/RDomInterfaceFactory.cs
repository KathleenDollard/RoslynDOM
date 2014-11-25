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
   public class RDomInterfaceTypeMemberFactory
     : RDomBaseSyntaxNodeFactory<RDomInterface, InterfaceDeclarationSyntax>
   {
      public RDomInterfaceTypeMemberFactory(RDomCorporation corporation)
          : base(corporation)
      { }

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

      protected override IEnumerable<IDom> CreateListFromInterim(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as InterfaceDeclarationSyntax;
         var newItem = new RDomInterface(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, whitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.TypeParameterList, LanguagePart.Current, whitespaceLookup);

         newItem.MembersAll.CreateAndAdd(syntax, x => x.Members, x => OutputContext.Corporation.Create(x, newItem, model).Cast<ITypeMemberAndDetail>());
         // this is a hack because the membersare appearing with a scope
         foreach (var member in newItem.MembersAll.OfType<ITypeMember>())
         { member.AccessModifier = AccessModifier.None; }

         return new IDom[] { newItem };
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IInterface;
         Guardian.Assert.IsNotNull(itemAsT, nameof(itemAsT));

         var modifiers = itemAsT.BuildModfierSyntax();
         var identifier = SyntaxFactory.Identifier(itemAsT.Name);

         var node = SyntaxFactory.InterfaceDeclaration(identifier)
             .WithModifiers(modifiers);

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

         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, whitespaceLookup);
         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }
   }
}
