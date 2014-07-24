using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomLockStatementFactory
                : RDomStatementFactory<RDomLockStatement, LockStatementSyntax>
    {
        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as LockStatementSyntax;
            var newItem = new RDomLockStatement(syntaxNode, parent, model);

            var expr = RDomFactoryHelper.GetHelperForExpression().MakeItems(syntax.Expression, newItem, model).FirstOrDefault();
            newItem.Expression = expr;

            bool hasBlock = false;
            var statements = CreateFromHelpers.GetStatementsFromSyntax(syntax.Statement, newItem, ref hasBlock, model);
            newItem.HasBlock = hasBlock;
            newItem.StatementsAll.AddOrMoveRange(statements);

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatementCommentWhite item)
        {
            var itemAsT = item as ILockStatement;
            var statement = RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT.HasBlock);
            var expressionSyntax = RDomCSharpFactory.Factory.BuildSyntax(itemAsT.Expression) as ExpressionSyntax;
            var node = SyntaxFactory.LockStatement(expressionSyntax, statement);

            return itemAsT.PrepareForBuildSyntaxOutput(node);
        }
    }
}
