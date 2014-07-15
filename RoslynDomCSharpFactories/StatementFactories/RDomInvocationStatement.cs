using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomInvocationStatementFactory
         : RDomStatementFactory<IInvocationStatement, ExpressionStatementSyntax>
    {
        public override void InitializeItem(IInvocationStatement newItem, ExpressionStatementSyntax syntax)
        {
            var expression = syntax.Expression;
            newItem.Invocation = RDomFactoryHelper.ExpressionFactoryHelper.MakeItem(expression).FirstOrDefault();
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IInvocationStatement;
            var expressionSyntax = RDomFactory.BuildSyntax(itemAsT.Invocation);
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
