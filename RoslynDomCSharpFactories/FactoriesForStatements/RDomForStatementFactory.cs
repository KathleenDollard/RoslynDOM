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
         : RDomStatementFactory<RDomForStatement, ForStatementSyntax>
    {
        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ForStatementSyntax;
            var newItem = new RDomForStatement(syntaxNode, parent, model);
            newItem.TestAtEnd = false;
            var decl = syntax.Declaration.Variables.First();
            var newVariable = RDomFactoryHelper.GetHelper<IMisc>().MakeItem(syntax.Declaration, newItem, model).FirstOrDefault();
            newItem.Variable = (IVariableDeclaration)newVariable;
            newItem.Incrementor = RDomFactoryHelper.GetHelper<IExpression>().MakeItem(syntax.Incrementors.First(), newItem, model).FirstOrDefault();
            return LoopFactoryHelper.CreateFrom<IForStatement>(newItem, syntax.Condition, syntax.Statement, parent, model);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IForStatement;
            var node = LoopFactoryHelper.BuildSyntax<IForStatement>(itemAsT, (c, s) => SyntaxFactory.ForStatement(s).WithCondition(c)).First() as ForStatementSyntax ;

            // TODO: Try to share this code with DeclarationSyntaxStatementFactory
            TypeSyntax typeSyntax;
            if (itemAsT.Variable.IsImplicitlyTyped)
            { typeSyntax = SyntaxFactory.IdentifierName("var"); }
            else
            { typeSyntax = (TypeSyntax)(RDomCSharpFactory.Factory.BuildSyntax(itemAsT.Variable.Type)); }
            var expressionSyntax = RDomCSharpFactory.Factory.BuildSyntax(itemAsT.Variable.Initializer);
            var nodeDeclarator = SyntaxFactory.VariableDeclarator(itemAsT.Variable.Name);
            nodeDeclarator = nodeDeclarator.WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax)expressionSyntax));
            var nodeDeclaratorInList = SyntaxFactory.SeparatedList(SyntaxFactory.List<VariableDeclaratorSyntax>(new VariableDeclaratorSyntax[] { (VariableDeclaratorSyntax)nodeDeclarator }));
            var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
            node = node.WithDeclaration(nodeDeclaration);

            var incrementorSyntax = RDomCSharpFactory.Factory.BuildSyntax(itemAsT.Incrementor );
           node = node.WithIncrementors(SyntaxFactory.SeparatedList<ExpressionSyntax>(new ExpressionSyntax[] {(ExpressionSyntax) incrementorSyntax }));

            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }


    }
}
