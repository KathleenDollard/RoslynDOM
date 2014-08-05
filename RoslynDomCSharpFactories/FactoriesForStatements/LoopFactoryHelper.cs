using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    internal static class LoopFactoryHelper
    {
        // until move to C# 6 - I want to support name of as soon as possible
        [ExcludeFromCodeCoverage]
        private static string nameof<T>(T value) { return ""; }

        public static IStatement CreateItemFrom<T>(
            T newItem, ExpressionSyntax condition, StatementSyntax statement, IDom parent, SemanticModel model,
            RDomCorporation corporation, ICreateFromWorker createFromWorker)
            where T : ILoop<T>
        {
            createFromWorker.InitializeStatements(newItem, statement, newItem, model);

            newItem.Condition = corporation.CreateFrom<IExpression>(condition, newItem, model).FirstOrDefault();
            Guardian.Assert.IsNotNull(condition, nameof(condition));

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
                var statement = RoslynCSharpUtilities.BuildStatement(item.Statements, item);
                var condition = RDomCSharp.Factory.BuildSyntax(item.Condition);
                node = makeSyntaxDelegate((ExpressionSyntax)condition, statement);
            }

            var leadingTrivia = BuildSyntaxHelpers.LeadingTrivia(item);
            node = node.WithLeadingTrivia(leadingTrivia);

            return node.PrepareForBuildSyntaxOutput(item);
        }
    }
}
