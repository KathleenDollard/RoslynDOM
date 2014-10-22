using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class RDomMethodTypeMemberFactory
         : RDomTypeMemberFactory<RDomMethod, MethodDeclarationSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomMethodTypeMemberFactory(RDomCorporation corporation)
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
               _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.Add(LanguageElement.ParameterStartDelimiter, SyntaxKind.OpenParenToken);
               _whitespaceLookup.Add(LanguageElement.ParameterEndDelimiter, SyntaxKind.CloseParenToken);
               _whitespaceLookup.Add(LanguageElement.NewSlot, SyntaxKind.NewKeyword);
               _whitespaceLookup.Add(LanguageElement.ThisForExtension, SyntaxKind.ThisKeyword);
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
         var syntax = syntaxNode as MethodDeclarationSyntax;
         var newItem = new RDomMethod(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
         CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.Body, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.ParameterList, LanguagePart.Current, WhitespaceLookup);

         newItem.Name = newItem.TypedSymbol.Name;

         var returnType = Corporation
                         .CreateFrom<IMisc>(syntax.ReturnType, newItem, model)
                         .FirstOrDefault()
                         as IReferencedType;
         newItem.ReturnType = returnType;

         newItem.IsExtensionMethod = newItem.TypedSymbol.IsExtensionMethod;

         var parameters = ListUtilities.MakeList(syntax, x => x.ParameterList.Parameters, x => Corporation.CreateFrom<IMisc>(x, newItem, model))
                             .OfType<IParameter>();
         newItem.Parameters.AddOrMoveRange(parameters);

         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IMethod;
         var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);

         var returnTypeSyntax = (TypeSyntax)RDom.CSharp.GetSyntaxGroup(itemAsT.ReturnType).First();
         var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
         var node = SyntaxFactory.MethodDeclaration(returnTypeSyntax, nameSyntax)
                         .WithModifiers(modifiers);
         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

         var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         var parameterList = itemAsT.Parameters
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .OfType<ParameterSyntax>()
                     .ToList();
         if (itemAsT.IsExtensionMethod)
         {
            // this this is a normal list, ref semantics
            var firstParam = parameterList.FirstOrDefault();
            parameterList.Remove(firstParam);
            if (firstParam == null) { throw new InvalidOperationException("Extension methods must have at least one parameter"); }
            // I'm cheating a bit here. Since the This keyword is an indicator of extension state on the method
            // I'm hardcoding a single space. I don't see the complexity of dealing with this as worth it unless
            // there's gnashing of teeth over this single space. The use of "this" on the parameter is not universal
            // and VB marks the method. 
            var thisModifier = SyntaxFactory.Token(SyntaxKind.ThisKeyword)
                                    .WithTrailingTrivia(SyntaxFactory.ParseTrailingTrivia(" "));
            var paramModifiers = firstParam.Modifiers.Insert(0, thisModifier);
            firstParam = firstParam.WithModifiers(paramModifiers);
            parameterList.Insert(0, firstParam);
         }
         var parameterListSyntax = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterList));
         parameterListSyntax = BuildSyntaxHelpers.AttachWhitespace(parameterListSyntax, itemAsT.Whitespace2Set, WhitespaceLookup);
         node = node.WithParameterList(parameterListSyntax);

         node = node.WithBody((BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup));

         // This works oddly because it uncollapses the list
         // This code is largely repeated in interface and class factories, but is very hard to refactor because of shallow Roslyn (Microsoft) architecture
         var typeParamsAndConstraints = itemAsT.TypeParameters
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .ToList();

         node = BuildSyntaxHelpers.BuildTypeParameterSyntax(
                 itemAsT, node, WhitespaceLookup,
                 (x, p) => x.WithTypeParameterList(p),
                 (x, c) => x.WithConstraintClauses(c));
         //var typeParameterSyntaxList = BuildSyntaxHelpers.GetTypeParameterSyntaxList(
         //            typeParamsAndConstraints, itemAsT.Whitespace2Set, WhitespaceLookup);
         //if (typeParameterSyntaxList != null)
         //{
         //    node = node.WithTypeParameterList(typeParameterSyntaxList);
         //    var clauses = BuildSyntaxHelpers.GetTypeParameterConstraintList(
         //              typeParamsAndConstraints, itemAsT.Whitespace2Set, WhitespaceLookup);
         //    if (clauses.Any())
         //    { node = node.WithConstraintClauses(clauses); }
         //}

         return node.PrepareForBuildSyntaxOutput(item);
      }

   }


}
