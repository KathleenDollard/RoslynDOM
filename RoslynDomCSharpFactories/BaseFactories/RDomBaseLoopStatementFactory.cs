using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
   public abstract class RDomBaseLoopStatementFactory<T, TSyntax>
        : RDomBaseSyntaxNodeFactory<T, TSyntax>
      where T : class, IDom, ILoop
      where TSyntax : SyntaxNode
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomBaseLoopStatementFactory(RDomCorporation corporation)
         : base(corporation)
      { }

      protected abstract TSyntax MakeSyntax(T itemAsT, ExpressionSyntax condition, StatementSyntax statementBlock);

      protected abstract T MakeNewItem(TSyntax syntaxNode, IDom parent, SemanticModel model);

      protected abstract ExpressionSyntax GetConditionFromSyntax(TSyntax syntaxNode);

      protected abstract StatementSyntax GetStatementFromSyntax(TSyntax syntax);

      protected WhitespaceKindLookup WhitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken); // For For Each
               _whitespaceLookup.Add(LanguageElement.DoKeyword, SyntaxKind.DoKeyword);
               _whitespaceLookup.Add(LanguageElement.WhileKeyword, SyntaxKind.WhileKeyword);
               _whitespaceLookup.Add(LanguageElement.ForKeyword, SyntaxKind.ForKeyword);
               _whitespaceLookup.Add(LanguageElement.ForEachKeyword, SyntaxKind.ForEachKeyword);
               _whitespaceLookup.Add(LanguageElement.EqualsAssignmentOperator, SyntaxKind.EqualsKeyword);
               _whitespaceLookup.Add(LanguageElement.InKeyword, SyntaxKind.InKeyword);
               _whitespaceLookup.Add(LanguageElement.ConditionalStartDelimiter, SyntaxKind.OpenParenToken);
               _whitespaceLookup.Add(LanguageElement.ConditionalEndDelimiter, SyntaxKind.CloseParenToken);
               _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as TSyntax;
         var newItem = MakeNewItem(syntax, parent, model);
         var condition = GetConditionFromSyntax(syntax);
         var statement = GetStatementFromSyntax(syntax);

         var conditionObj = OutputContext.Corporation.CreateSpecial<IExpression>(condition, newItem, model).FirstOrDefault();
         newItem.Condition = conditionObj as IExpression;
         CreateFromWorker.InitializeStatements(newItem, statement, newItem, model);

         Guardian.Assert.IsNotNull(condition, nameof(condition));

         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, statement, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, condition, LanguagePart.Current, WhitespaceLookup);

         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as T;
         SyntaxNode node;
         if (itemAsT.Condition == null)
         // TODO: Isn't condition null in a ForEach?
         { node = SyntaxFactory.EmptyStatement(); }// This shold not happen
         else
         {
            var statement = RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup);
            var condition = (ExpressionSyntax)RDom.CSharp.GetSyntaxNode(itemAsT.Condition);
            node = MakeSyntax(itemAsT, condition, statement);
         }

         var leadingTrivia = BuildSyntaxHelpers.LeadingTrivia(itemAsT, OutputContext);
         node = node.WithLeadingTrivia(leadingTrivia);
         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
         node = AdjustWhitespace(node, itemAsT, WhitespaceLookup);

         return node.PrepareForBuildSyntaxOutput(itemAsT, OutputContext);
      }

      protected virtual SyntaxNode AdjustWhitespace(SyntaxNode node, T item, WhitespaceKindLookup WhitespaceLookup)
      {
         return node;
      }
   }
}