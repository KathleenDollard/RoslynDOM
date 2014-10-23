using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class RDomConversionOperatorTypeMemberFactory
         : RDomBaseItemFactory<RDomConversionOperator, ConversionOperatorDeclarationSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomConversionOperatorTypeMemberFactory(RDomCorporation corporation)
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
               _whitespaceLookup.Add(LanguageElement.OperatorKeyword, SyntaxKind.OperatorKeyword);
               _whitespaceLookup.Add(LanguageElement.ExplicitConversion, SyntaxKind.ExplicitKeyword);
               _whitespaceLookup.Add(LanguageElement.ImplicitConversion, SyntaxKind.ImplicitKeyword);
               _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.Add(LanguageElement.ParameterStartDelimiter, SyntaxKind.OpenParenToken);
               _whitespaceLookup.Add(LanguageElement.ParameterEndDelimiter, SyntaxKind.CloseParenToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.StaticModifiers);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as ConversionOperatorDeclarationSyntax;
         var newItem = new RDomConversionOperator(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
         CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);
         CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.Body, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.ParameterList, LanguagePart.Current, WhitespaceLookup);

         newItem.Name = newItem.TypedSymbol.Name;

         var type = Corporation
                         .Create(syntax.Type, newItem, model)
                         .FirstOrDefault()
                         as IReferencedType;
         newItem.Type = type;

         newItem.IsStatic = newItem.Symbol.IsStatic;
         newItem.IsImplicit = (syntax.ImplicitOrExplicitKeyword.CSharpKind() == SyntaxKind.ImplicitKeyword);

         newItem.Parameters.CreateAndAdd(syntax, x => x.ParameterList.Parameters, x => Corporation.Create(x, newItem, model).Cast<IParameter>());
         //var parameters = ListUtilities.MakeList(syntax, x => x.ParameterList.Parameters, x => Corporation.Create(x, newItem, model))
         //                       .OfType<IParameter>();
         //newItem.Parameters.AddOrMoveRange(parameters);

         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IConversionOperator;

         var kind = itemAsT.IsImplicit ? SyntaxKind.ImplicitKeyword : SyntaxKind.ExplicitKeyword;
         var opToken = SyntaxFactory.Token(kind);
         var typeSyntax = (TypeSyntax)RDom.CSharp.GetSyntaxGroup(itemAsT.Type).First();
         var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
         var node = SyntaxFactory.ConversionOperatorDeclaration(opToken, typeSyntax)
                         .WithModifiers(modifiers);
         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

         var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
         if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

         //node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

         var parameterList = itemAsT.Parameters
                     .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                     .OfType<ParameterSyntax>()
                     .ToList();
         var parameterListSyntax = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterList));
         parameterListSyntax = BuildSyntaxHelpers.AttachWhitespace(parameterListSyntax, itemAsT.Whitespace2Set, WhitespaceLookup);
         node = node.WithParameterList(parameterListSyntax);

         node = node.WithBody((BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup));

         return node.PrepareForBuildSyntaxOutput(item);
      }

   }


}
