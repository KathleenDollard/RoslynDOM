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
            RDomCorporation corporation, ICSharpCreateFromWorker createFromWorker, WhitespaceKindLookup whitespaceLookup)
            where T : ILoop<T>
        {
            newItem.Condition = corporation.CreateFrom<IExpression>(condition, newItem, model).FirstOrDefault();
            createFromWorker.InitializeStatements(newItem, statement, newItem, model);

            Guardian.Assert.IsNotNull(condition, nameof(condition));

            createFromWorker.StoreWhitespace(newItem, statement, LanguagePart.Block, whitespaceLookup);
            createFromWorker.StoreWhitespace(newItem, condition, LanguagePart.Current, whitespaceLookup);

            return newItem;
        }

        public static IEnumerable<SyntaxNode> BuildSyntax<T>
            (T itemAsT, Func<ExpressionSyntax, StatementSyntax, SyntaxNode> makeSyntaxDelegate,
            WhitespaceKindLookup whitespaceLookup)
            where T : ILoop<T>
        {

            SyntaxNode node;
            if (itemAsT.Condition == null)
            // TODO: Isn't conditin null in a ForEach?
            { node = SyntaxFactory.EmptyStatement(); }// This shold not happen 
            else
            {
                var statement = RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, whitespaceLookup);
                var condition = RDomCSharp.Factory.BuildSyntax(itemAsT.Condition);
                node = makeSyntaxDelegate((ExpressionSyntax)condition, statement);
            }

            var leadingTrivia = BuildSyntaxHelpers.LeadingTrivia(itemAsT);
            node = node.WithLeadingTrivia(leadingTrivia);
            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, whitespaceLookup);

            return node.PrepareForBuildSyntaxOutput(itemAsT);
        }
    }
}
