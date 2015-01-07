using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
   public class RDomLockStatementFactory
               : RDomBaseSyntaxNodeFactory<RDomLockStatement, LockStatementSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomLockStatementFactory(RDomCorporation corporation)
         : base(corporation)
      { }

      private WhitespaceKindLookup WhitespaceLookup
      {
         get
         {
            if (_whitespaceLookup == null)
            {
               _whitespaceLookup = new WhitespaceKindLookup();
               _whitespaceLookup.Add(LanguageElement.Locked, SyntaxKind.LockKeyword);
               _whitespaceLookup.Add(LanguageElement.VariableStartDelimiter, SyntaxKind.OpenParenToken);
               _whitespaceLookup.Add(LanguageElement.VariableEndDelimiter, SyntaxKind.CloseParenToken);
               _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
               _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
            }
            return _whitespaceLookup;
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var syntax = syntaxNode as LockStatementSyntax;
         var newItem = new RDomLockStatement(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
         CreateFromWorker.InitializeStatements(newItem, syntax.Statement, newItem, model);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.Statement, LanguagePart.Current, WhitespaceLookup);

         var expr = OutputContext.Corporation.CreateSpecial<IExpression>(syntax.Expression, newItem, model).FirstOrDefault();
         CreateFromWorker.StoreWhitespace(expr, syntax.Expression, LanguagePart.Current, WhitespaceLookup);
         newItem.Expression = expr;

         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as ILockStatement;
         var statement = RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup);
         var expressionSyntax = RDom.CSharp.GetSyntaxNode(itemAsT.Expression) as ExpressionSyntax;
         var node = SyntaxFactory.LockStatement(expressionSyntax, statement);

         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
         return node.PrepareForBuildSyntaxOutput(item, OutputContext);
      }
   }
}