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
        //private static WhitespaceKindLookup _whitespaceLookup;
        private TriviaManager triviaManager = new TriviaManager();

        public RDomForStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        //private WhitespaceKindLookup WhitespaceLookup
        //{
        //    get
        //    {
        //        if (_whitespaceLookup == null)
        //        {
        //            _whitespaceLookup = new WhitespaceKindLookup();
        //            _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
        //        }
        //        return _whitespaceLookup;
        //    }
        //}

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
            newItem.Incrementor = Corporation.CreateFrom<IExpression>(syntax.Incrementors.First(), newItem, model).FirstOrDefault();
            return newItem;

            //var syntax = syntaxNode as ForStatementSyntax;
            //var newItem = new RDomForStatement(syntaxNode, parent, model);
            //CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            //CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

            //newItem.TestAtEnd = false;
            //var decl = syntax.Declaration.Variables.First();
            //var newVariable = Corporation.CreateFrom<IMisc>(syntax.Declaration, newItem, model).FirstOrDefault();
            //newItem.Variable = (IVariableDeclaration)newVariable;
            //newItem.Incrementor = Corporation.CreateFrom<IExpression>(syntax.Incrementors.First(), newItem, model).FirstOrDefault();
            //return LoopFactoryHelper.CreateItemFrom<IForStatement>(newItem,
            //    syntax.Condition, syntax.Statement, parent, model,
            //        Corporation, CreateFromWorker, WhitespaceLookup);
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
            var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(itemAsT.Variable);
            //var expressionSyntax = (ExpressionSyntax)RDomCSharp.Factory.BuildSyntax(itemAsT.Variable.Initializer);
            //var nodeDeclarator = SyntaxFactory.VariableDeclarator(itemAsT.Variable.Name);
            //nodeDeclarator = nodeDeclarator.WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax)expressionSyntax));
            var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { declaratorSyntax }));
            var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
            nodeDeclaration = BuildSyntaxHelpers.AttachWhitespace(nodeDeclaration, itemAsT.Whitespace2Set, WhitespaceLookup, LanguagePart.Variable);
            var incrementorSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Incrementor);
            incrementorSyntax = BuildSyntaxHelpers.AttachWhitespace(incrementorSyntax, itemAsT.Whitespace2Set, WhitespaceLookup, LanguagePart.Incrementor);

            //var firstSemiColonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken);
            //firstSemiColonToken = triviaManager.AttachWhitespaceToToken(firstSemiColonToken, itemAsT.Whitespace2Set[LanguagePart.Variable, LanguageElement.EndOfLine]);
            var secondSemiColonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken);
            secondSemiColonToken = triviaManager.AttachWhitespaceToToken(secondSemiColonToken, itemAsT.Whitespace2Set[LanguagePart.Condition, LanguageElement.EndOfLine]);

            var node = SyntaxFactory.ForStatement(statementBlock)
                 .WithCondition(condition)
                 .WithDeclaration(nodeDeclaration)
                 .WithIncrementors(SyntaxFactory.SeparatedList<ExpressionSyntax>(new ExpressionSyntax[] { (ExpressionSyntax)incrementorSyntax }))
                 .WithSecondSemicolonToken(secondSemiColonToken);

            return node;
        }

        //public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        //{
        //    var itemAsT = item as IForStatement;
        //    var node = LoopFactoryHelper.BuildSyntax<IForStatement>(itemAsT,
        //        (c, s) => SyntaxFactory.ForStatement(s).WithCondition(c), WhitespaceLookup)
        //        .First() as ForStatementSyntax;

        //    var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(itemAsT.Variable);
        //    //// TODO: Try to share this code with DeclarationSyntaxStatementFactory
        //    //TypeSyntax typeSyntax;
        //    //if (itemAsT.Variable.IsImplicitlyTyped)
        //    //{ typeSyntax = SyntaxFactory.IdentifierName("var"); }
        //    //else
        //    //{ typeSyntax = (TypeSyntax)(RDomCSharp.Factory.BuildSyntax(itemAsT.Variable.Type)); }
        //    var expressionSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Variable.Initializer);
        //    var nodeDeclarator = SyntaxFactory.VariableDeclarator(itemAsT.Variable.Name);
        //    nodeDeclarator = nodeDeclarator.WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax)expressionSyntax));
        //    var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { (VariableDeclaratorSyntax)nodeDeclarator }));
        //    var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
        //    node = node.WithDeclaration(nodeDeclaration);

        //    var incrementorSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Incrementor);
        //    node = node.WithIncrementors(SyntaxFactory.SeparatedList<ExpressionSyntax>(new ExpressionSyntax[] { (ExpressionSyntax)incrementorSyntax }));

        //    node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
        //    return node.PrepareForBuildSyntaxOutput(item);
        //}


    }
}
