using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomInvocationStatementFactory
         : RDomStatementFactory<RDomInvocationStatement, ExpressionStatementSyntax>
    {
        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ExpressionStatementSyntax;
            var newItem = new RDomInvocationStatement(syntaxNode, parent, model);

            var expression = syntax.Expression;
            newItem.Invocation = RDomFactoryHelper.GetHelper<IExpression>().MakeItem(expression, newItem, model).FirstOrDefault();

            return new IStatement[] { newItem };
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IInvocationStatement;
            var expressionSyntax = RDomCSharpFactory.Factory.BuildSyntax(itemAsT.Invocation);
            var node = SyntaxFactory.ExpressionStatement((ExpressionSyntax)expressionSyntax);
            return new SyntaxNode[] { RoslynUtilities.Format(node) };

        }

        public override FactoryPriority Priority
        {
            get { return FactoryPriority.Normal - 1; }
        }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return syntaxNode is ExpressionStatementSyntax;
        }
    }
}
