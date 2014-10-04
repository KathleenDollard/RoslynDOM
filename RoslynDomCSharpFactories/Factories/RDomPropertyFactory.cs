using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class RDomPropertyTypeMemberFactory
         : RDomTypeMemberFactory<RDomProperty, PropertyDeclarationSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomPropertyTypeMemberFactory(RDomCorporation corporation)
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
               _whitespaceLookup.Add(LanguageElement.AccessorGroupStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.AccessorGroupEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.Add(LanguageElement.NewSlot, SyntaxKind.NewKeyword);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.OopModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.StaticModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }
      protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as PropertyDeclarationSyntax;
         var newItem = new RDomProperty(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
         CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.AccessorList, LanguagePart.Current, WhitespaceLookup);

         newItem.Name = newItem.TypedSymbol.Name;

         var type = Corporation
                         .CreateFrom<IMisc>(syntax.Type, newItem, model)
                         .FirstOrDefault()
                         as IReferencedType;
         newItem.PropertyType = type;

         var propSymbol = newItem.Symbol as IPropertySymbol;
         Guardian.Assert.IsNotNull(propSymbol, nameof(propSymbol));

         newItem.CanGet = (!propSymbol.IsWriteOnly); // or check whether getAccessor is null
         newItem.CanSet = (!propSymbol.IsReadOnly); // or check whether setAccessor is null
         var getAccessorSyntax = syntax.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.GetAccessorDeclaration).FirstOrDefault();
         var setAccessorSyntax = syntax.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.SetAccessorDeclaration).FirstOrDefault();
         if (getAccessorSyntax != null)
         { newItem.GetAccessor = (IAccessor)Corporation.CreateFrom<IMisc>(getAccessorSyntax, newItem, model).FirstOrDefault(); }
         if (setAccessorSyntax != null)
         { newItem.SetAccessor = (IAccessor)Corporation.CreateFrom<IMisc>(setAccessorSyntax, newItem, model).FirstOrDefault(); }

         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IProperty;
         var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);
         var returnType = (TypeSyntax)RDomCSharp.Factory.BuildSyntaxGroup(itemAsT.ReturnType).First();
         var node = SyntaxFactory.PropertyDeclaration(returnType, nameSyntax);
         var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
         node = node.WithModifiers(modifiers);

         var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         var accessors = SyntaxFactory.List<AccessorDeclarationSyntax>();
         var getAccessorSyntax = RDomCSharp.Factory.BuildSyntaxGroup(itemAsT.GetAccessor).FirstOrDefault();
         if (getAccessorSyntax != null) { accessors = accessors.Add((AccessorDeclarationSyntax)getAccessorSyntax); }

         var setAccessorSyntax = RDomCSharp.Factory.BuildSyntaxGroup(itemAsT.SetAccessor).FirstOrDefault();
         if (setAccessorSyntax != null) { accessors = accessors.Add((AccessorDeclarationSyntax)setAccessorSyntax); }

         var accessorList = SyntaxFactory.AccessorList(accessors);
         accessorList = BuildSyntaxHelpers.AttachWhitespace(accessorList, itemAsT.Whitespace2Set, WhitespaceLookup);
         if (accessors.Any()) { node = node.WithAccessorList(accessorList); }

         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

         return node.PrepareForBuildSyntaxOutput(item);
      }
   }

}
