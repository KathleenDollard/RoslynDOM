using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomForStatementFactory
         : RDomBaseLoopStatementFactory<RDomForStatement, ForStatementSyntax>
    {
        private TriviaManager triviaManager = new TriviaManager();

        public RDomForStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ForStatementSyntax;
            var newItem = base.CreateItemFrom(syntaxNode, parent, model) as RDomForStatement;
            newItem.TestAtEnd = false;
            var decl = syntax.Declaration.Variables.First();
            var newVariable = Corporation.CreateFrom<IMisc>(syntax, newItem, model).FirstOrDefault();
            newItem.Variable = (IVariableDeclaration)newVariable;
            newItem.Whitespace2Set.AddRange(newItem.Variable.Whitespace2Set
                                            .Select(x => new Whitespace2(LanguagePart.Variable, x.LanguageElement,
                                                    x.LeadingWhitespace, x.TrailingWhitespace, x.TrailingComment)));
            SetWhitespaceForSemiColon(syntax, 1, newItem, LanguagePart.Variable);
            SetWhitespaceForSemiColon(syntax, 2, newItem, LanguagePart.Condition);
            newItem.Iterator = Corporation.CreateFrom<IExpression>(syntax.Incrementors.First(), newItem, model).FirstOrDefault();
            return newItem;

        }

        private void SetWhitespaceForSemiColon(ForStatementSyntax syntax, int semiColonNumber,
                    RDomForStatement newItem, LanguagePart languagePart)
        {
            var token = semiColonNumber == 0
                            ? syntax.FirstSemicolonToken
                            : syntax.SecondSemicolonToken;
            var whitespace = new Whitespace2(languagePart, LanguageElement.EndOfLine,
                        token.LeadingTrivia.ToString(), token.TrailingTrivia.ToString(), null);
            newItem.Whitespace2Set.Add(whitespace);
        }

        protected override ExpressionSyntax GetConditionFromSyntax(ForStatementSyntax syntax)
        {
            return syntax.Condition; ;
        }

        protected override StatementSyntax GetStatementFromSyntax(ForStatementSyntax syntax)
        {
            return syntax.Statement;
        }

        protected override RDomForStatement MakeNewItem(ForStatementSyntax syntax, IDom parent, SemanticModel model)
        {
            return new RDomForStatement(syntax, parent, model);
        }

        protected override ForStatementSyntax MakeSyntax(RDomForStatement itemAsT, ExpressionSyntax condition, StatementSyntax statementBlock)
        {
            var declaratorSyntax = (VariableDeclaratorSyntax)RDomCSharp.Factory.BuildSyntax(itemAsT.Variable);
            var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(
                        itemAsT.Variable.IsImplicitlyTyped, itemAsT.Variable.Type);
            //var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(itemAsT.Variable);
            var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { declaratorSyntax }));
            var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
            nodeDeclaration = BuildSyntaxHelpers.AttachWhitespace(nodeDeclaration, itemAsT.Whitespace2Set, WhitespaceLookup, LanguagePart.Variable);
            var incrementorSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Iterator);
            incrementorSyntax = BuildSyntaxHelpers.AttachWhitespace(incrementorSyntax, itemAsT.Whitespace2Set, WhitespaceLookup, LanguagePart.Iterator);

            var secondSemiColonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken);
            secondSemiColonToken = triviaManager.AttachWhitespaceToToken(secondSemiColonToken, itemAsT.Whitespace2Set[LanguagePart.Condition, LanguageElement.EndOfLine]);

            var node = SyntaxFactory.ForStatement(statementBlock)
                 .WithCondition(condition)
                 .WithDeclaration(nodeDeclaration)
                 .WithIncrementors(SyntaxFactory.SeparatedList<ExpressionSyntax>(new ExpressionSyntax[] { (ExpressionSyntax)incrementorSyntax }))
                 .WithSecondSemicolonToken(secondSemiColonToken);

            return node;
        }

     }
}
