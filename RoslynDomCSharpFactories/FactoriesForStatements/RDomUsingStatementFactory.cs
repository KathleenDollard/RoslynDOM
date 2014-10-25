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
    public class RDomUsingStatementFactory
                : RDomBaseItemFactory<RDomUsingStatement, UsingStatementSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomUsingStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.Add(LanguageElement.Using, SyntaxKind.UsingKeyword);
                    _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
                    _whitespaceLookup.Add(LanguageElement.VariableStartDelimiter, SyntaxKind.OpenParenToken);
                    _whitespaceLookup.Add(LanguageElement.VariableEndDelimiter, SyntaxKind.CloseParenToken);
                    _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
                    _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
                    _whitespaceLookup.Add(LanguageElement.EqualsAssignmentOperator, SyntaxKind.EqualsToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as UsingStatementSyntax;
            var newItem = new RDomUsingStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Statement, newItem, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, syntax.Statement, LanguagePart.Current, WhitespaceLookup);
            // if there is both a declaration and an expression, I'm terribly confused
            var declaration = syntax.Declaration;
            var expression = syntax.Expression;
            if (declaration != null && expression != null) throw new InvalidOperationException();
            if (declaration == null && expression == null) throw new InvalidOperationException();
            var statement = syntax.Statement;
            if (declaration != null)
            {
                // Not yet, and might never support as Kendall Miller said "huh, that works?" on Twitter
                if (declaration.Variables.Count() > 1) throw new NotImplementedException();

                var newVariable = OutputContext.Corporation.Create(syntax.Declaration, newItem, model).FirstOrDefault();
                newItem.Variable = (IVariableDeclaration)newVariable;
            }
            else
            {
                var expr = OutputContext.Corporation.Create<IExpression>(syntax.Expression, newItem, model).FirstOrDefault();
                CreateFromWorker.StoreWhitespace(expr, syntax.Expression, LanguagePart.Current, WhitespaceLookup);
                newItem.Expression = expr;
            }

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IUsingStatement;
            var statement = RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup);
            var node = SyntaxFactory.UsingStatement(statement);
            if (itemAsT.Variable != null)
            {
                //var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(itemAsT.Variable);
                var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(
                            itemAsT.Variable.IsImplicitlyTyped, itemAsT.Variable.Type);
                var expressionSyntax = RDom.CSharp.GetSyntaxNode(itemAsT.Variable.Initializer);
                var equalsValueClause = SyntaxFactory.EqualsValueClause((ExpressionSyntax)expressionSyntax);
                equalsValueClause = BuildSyntaxHelpers.AttachWhitespace(equalsValueClause, itemAsT.Variable.Whitespace2Set, WhitespaceLookup);

                var nodeDeclarator = SyntaxFactory.VariableDeclarator(itemAsT.Variable.Name);
                nodeDeclarator = nodeDeclarator.WithInitializer(equalsValueClause);
                nodeDeclarator = BuildSyntaxHelpers.AttachWhitespace(nodeDeclarator, itemAsT.Variable.Whitespace2Set, WhitespaceLookup);
                var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { (VariableDeclaratorSyntax)nodeDeclarator }));
                var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
                node = node.WithDeclaration(nodeDeclaration);
            }
            else
            {
                var expressionSyntax = RDom.CSharp.GetSyntaxNode(itemAsT.Expression) as ExpressionSyntax;
                node = node.WithExpression(expressionSyntax);
            }

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            return node.PrepareForBuildSyntaxOutput(item, OutputContext);
        }
    }
}
