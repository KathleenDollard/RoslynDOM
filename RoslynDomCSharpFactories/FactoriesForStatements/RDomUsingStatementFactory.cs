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
                : RDomStatementFactory<RDomUsingStatement, UsingStatementSyntax>
    {
        public RDomUsingStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as UsingStatementSyntax;
            var newItem = new RDomUsingStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
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

                var newVariable = Corporation.CreateFrom<IMisc>(syntax.Declaration, newItem, model).FirstOrDefault();
                newItem.Variable = (IVariableDeclaration)newVariable;
            }
            else
            {
                var expr = Corporation.CreateFrom<IExpression>(syntax.Expression, newItem, model).FirstOrDefault();
                newItem.Expression = expr;
            }

              return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IUsingStatement;
            var statement = RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT.HasBlock);
            var node = SyntaxFactory.UsingStatement(statement);
            if (itemAsT.Variable != null)
            {
                TypeSyntax typeSyntax;
                // TODO: Try to move this to BuildSyntaxExtensions, shared with at least ForStatement, and probably VariableDeclarationStatement, perhaps call through IOC
                if (itemAsT.Variable.IsImplicitlyTyped)
                { typeSyntax = SyntaxFactory.IdentifierName("var"); }
                else
                { typeSyntax = (TypeSyntax)(RDomCSharp.Factory.BuildSyntax(itemAsT.Variable.Type)); }
                var expressionSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Variable.Initializer);
                var nodeDeclarator = SyntaxFactory.VariableDeclarator(itemAsT.Variable.Name);
                nodeDeclarator = nodeDeclarator.WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax)expressionSyntax));
                var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { (VariableDeclaratorSyntax)nodeDeclarator }));
                var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
                node = node.WithDeclaration(nodeDeclaration);
            }
            else
            {
                var expressionSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Expression) as ExpressionSyntax;
                node = node.WithExpression(expressionSyntax);
            }

            return itemAsT.PrepareForBuildSyntaxOutput(node);
        }
    }
}
