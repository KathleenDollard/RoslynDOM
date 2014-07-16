using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomReturnStatementFactory
                : RDomStatementFactory<RDomReturnStatement, ReturnStatementSyntax>
    {
        public override void InitializeItem(RDomReturnStatement newItem, ReturnStatementSyntax syntax)
        {
            if (syntax.Expression != null)
            {
                newItem.Return = RDomFactoryHelper.GetHelper<IExpression>().MakeItem(syntax.Expression).FirstOrDefault();
                if (newItem.Return == null) throw new InvalidOperationException();
            }
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IReturnStatement;
            var node = SyntaxFactory.ReturnStatement();
            if (itemAsT.Return != null)
            {
                var returnExpressionSyntax = RDomFactory.BuildSyntax(itemAsT.Return);
                node = node.WithExpression((ExpressionSyntax)returnExpressionSyntax);
            }
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }
}
