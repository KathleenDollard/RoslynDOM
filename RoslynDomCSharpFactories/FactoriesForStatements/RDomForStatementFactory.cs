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
        public RDomForStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ForStatementSyntax;
            var newItem = new RDomForStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            newItem.TestAtEnd = false;
            var decl = syntax.Declaration.Variables.First();
            var newVariable = Corporation.CreateFrom<IMisc>(syntax.Declaration, newItem, model).FirstOrDefault();
            newItem.Variable = (IVariableDeclaration)newVariable;
            newItem.Incrementor = Corporation.CreateFrom<IExpression>(syntax.Incrementors.First(), newItem, model).FirstOrDefault();
            return LoopFactoryHelper.CreateItemFrom<IForStatement>(newItem, syntax.Condition, syntax.Statement, parent, model, Corporation, CreateFromWorker);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IForStatement;
            var node = LoopFactoryHelper.BuildSyntax<IForStatement>(itemAsT, (c, s) => SyntaxFactory.ForStatement(s).WithCondition(c)).First() as ForStatementSyntax;

            // TODO: Try to share this code with DeclarationSyntaxStatementFactory
            TypeSyntax typeSyntax;
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

            var incrementorSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Incrementor);
            node = node.WithIncrementors(SyntaxFactory.SeparatedList<ExpressionSyntax>(new ExpressionSyntax[] { (ExpressionSyntax)incrementorSyntax }));

            return item.PrepareForBuildSyntaxOutput(node);
        }


    }
}
