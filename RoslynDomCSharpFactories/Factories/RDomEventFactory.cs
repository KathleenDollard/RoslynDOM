using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class RDomEventTypeMemberFactory
         : RDomTypeMemberFactory<RDomEvent, EventFieldDeclarationSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomEventTypeMemberFactory(RDomCorporation corporation)
       : base(corporation)
      { }

      private WhitespaceKindLookup WhitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
               _whitespaceLookup.Add(LanguageElement.Event, SyntaxKind.EventKeyword);
               _whitespaceLookup.Add(LanguageElement.NewSlot, SyntaxKind.NewKeyword);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.OopModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.StaticModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IEnumerable<ITypeMemberCommentWhite> CreateListFrom(
         SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var list = new List<ITypeMember>();

         var publicAnnotations = CreateFromWorker.GetPublicAnnotations(syntaxNode, parent, model);
         var rawEvent = syntaxNode as EventFieldDeclarationSyntax;
         var declarators = rawEvent.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
         foreach (var decl in declarators)
         {
            var newItem = new RDomEvent(decl, parent, model);
            list.Add(newItem);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, decl, LanguagePart.Current, WhitespaceLookup);

            newItem.Name = newItem.TypedSymbol.Name;

            if (decl.Initializer != null)
            {
               CreateFromWorker.StoreWhitespaceForToken(newItem, decl.Initializer.EqualsToken, LanguagePart.Current, LanguageElement.EqualsAssignmentOperator);
               CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, decl.Initializer, LanguagePart.Current, LanguageElement.Expression);
            }

            var type = Corporation
                             .CreateFrom<IMisc>(rawEvent.Declaration.Type, newItem, model)
                             .FirstOrDefault()
                             as IReferencedType;
            newItem.Type = type;

            var eventSymbol = newItem.Symbol as IEventSymbol;
            newItem.IsStatic = eventSymbol.IsStatic;
            // See note on IsNew on interface before changing
            newItem.IsNew = rawEvent.Modifiers.Any(x => x.CSharpKind() == SyntaxKind.NewKeyword);
            newItem.PublicAnnotations.Add(publicAnnotations);

         }
         return list;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IEvent;
         var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);

         var typeSyntax = (TypeSyntax)RDom.CSharp.GetSyntaxGroup(itemAsT.Type).First();
         var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
         var variableNode = SyntaxFactory.VariableDeclarator(nameSyntax);
         var variableNodes = SyntaxFactory.SeparatedList(new VariableDeclaratorSyntax[] { variableNode });
         var eventNode = SyntaxFactory.VariableDeclaration(typeSyntax, variableNodes);
         eventNode = BuildSyntaxHelpers.AttachWhitespace(eventNode, itemAsT.Whitespace2Set, WhitespaceLookup);

         var node = SyntaxFactory.EventFieldDeclaration(eventNode)
                        .WithModifiers(modifiers);
         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

         var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         return node.PrepareForBuildSyntaxOutput(item);
      }

   }


}
