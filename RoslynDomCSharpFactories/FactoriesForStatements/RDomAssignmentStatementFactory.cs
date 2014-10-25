using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class RDomAssignmentStatementFactory
        : RDomBaseSyntaxNodeFactory<RDomAssignmentStatement, ExpressionStatementSyntax>
   {
      private static WhitespaceKindLookup _whitespaceLookup;

      public RDomAssignmentStatementFactory(RDomCorporation corporation)
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
               _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
               //_whitespaceLookup.AddRange(WhitespaceKindLookup.AssignmentOperators);
            }
            return _whitespaceLookup;
         }
      }

      public override RDomPriority Priority
      { get { return RDomPriority.Normal + 1; } }

      public override Func<SyntaxNode, IDom, SemanticModel, bool> CanCreateDelegate
      {
         get
         {
            return (syntax, parent, model) =>
                        {
                           var statement = syntax as ExpressionStatementSyntax;
                           if (statement == null) { return false; }
                           return (statement.Expression is BinaryExpressionSyntax);
                        };
         }
      }

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         // TODO: Why not cast immediately to BinaryExpression?
         var syntax = syntaxNode as ExpressionStatementSyntax;
         var newItem = new RDomAssignmentStatement(syntaxNode, parent, model);
         CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
         CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
         CreateFromWorker.StoreWhitespace(newItem, syntax.Expression, LanguagePart.Current, WhitespaceLookup);

         var binary = syntax.Expression as BinaryExpressionSyntax;
         Guardian.Assert.IsNotNull(binary, nameof(binary));

         var left = binary.Left as ExpressionSyntax;
         CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, left, LanguagePart.Current,
                 LanguageElement.LeftExpression);
         newItem.Left = OutputContext.Corporation.Create<IExpression>(left, newItem, model).FirstOrDefault();

         // Previously tested for identifier here, but can also be SimpleMemberAccess and ElementAccess expressions
         // not currently seeing value in testing for the type. Fix #46
         // Also changed Name to Left and string to expression
         var right = binary.Right;
         var expression = right as ExpressionSyntax;
         CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, expression, LanguagePart.Current,
                 LanguageElement.Expression);
         Guardian.Assert.IsNotNull(expression, nameof(expression));
         newItem.Expression = OutputContext.Corporation.Create<IExpression>(expression, newItem, model).FirstOrDefault();

         CreateFromWorker.StoreWhitespaceForToken(newItem, binary.OperatorToken,
                     LanguagePart.Current, LanguageElement.AssignmentOperator);
         newItem.Operator = Mappings.AssignmentOperatorFromCSharpKind(binary.CSharpKind());
         return newItem;
      }

      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         var itemAsT = item as IAssignmentStatement;
         var leftSyntax = RDom.CSharp.GetSyntaxNode(itemAsT.Left);
         leftSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(leftSyntax,
                     itemAsT.Whitespace2Set[LanguageElement.LeftExpression]);

         var expressionSyntax = RDom.CSharp.GetSyntaxNode(itemAsT.Expression);
         expressionSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(expressionSyntax,
                     itemAsT.Whitespace2Set[LanguageElement.Expression]);

         var syntaxKind = Mappings.SyntaxKindFromAssignmentOperator(itemAsT.Operator);
         var opToken = SyntaxFactory.Token(Mappings.SyntaxTokenKindFromAssignmentOperator(itemAsT.Operator));
         opToken = BuildSyntaxHelpers.AttachWhitespaceToToken(opToken, itemAsT.Whitespace2Set[LanguageElement.AssignmentOperator]);

         var assignmentSyntax = SyntaxFactory.BinaryExpression(syntaxKind,
                         (ExpressionSyntax)leftSyntax,
                         opToken,
                         (ExpressionSyntax)expressionSyntax);
         assignmentSyntax = BuildSyntaxHelpers.AttachWhitespace(assignmentSyntax, itemAsT.Whitespace2Set, WhitespaceLookup);
         var node = SyntaxFactory.ExpressionStatement(assignmentSyntax);

         node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
         return node.PrepareForBuildSyntaxOutput(item, OutputContext);

      }



   }
}
