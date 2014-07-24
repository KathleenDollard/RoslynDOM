using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    internal class LoopFactoryHelper
    {
        public static IStatement CreateItemFrom<T>(
            T newItem, ExpressionSyntax condition, StatementSyntax statement, IDom parent, SemanticModel model)
            where T : ILoop<T>
        {

            newItem.Condition = RDomFactoryHelper.GetHelperForExpression().MakeItems(condition, newItem, model).FirstOrDefault();
            if (condition == null) { throw new InvalidOperationException(); }
            bool hasBlock = false;
            var statements = RoslynCSharpUtilities.GetStatementsFromSyntax(statement, newItem, ref hasBlock, model);
            newItem.HasBlock = hasBlock;
            newItem.StatementsAll.AddOrMoveRange(statements);

            return newItem;
        }

        public static IEnumerable<SyntaxNode> BuildSyntax<T>
            (T item, Func<ExpressionSyntax, StatementSyntax, SyntaxNode> makeSyntaxDelegate)
            where T : ILoop<T>
        {

            SyntaxNode node;
            if (item.Condition == null)
            // TODO: Isn't conditin null in a ForEach?
            { node = SyntaxFactory.EmptyStatement(); }// This shold not happen 
            else
            {
                var statement = RoslynCSharpUtilities.BuildStatement(item.Statements, item.HasBlock);
                var condition = RDomCSharpFactory.Factory.BuildSyntax(item.Condition);
                node = makeSyntaxDelegate((ExpressionSyntax)condition, statement);
            }

            var leadingTrivia = BuildSyntaxExtensions.LeadingTrivia(item);
            node = node.WithLeadingTrivia(leadingTrivia);

            return item.PrepareForBuildSyntaxOutput(node);
        }
    }
}
