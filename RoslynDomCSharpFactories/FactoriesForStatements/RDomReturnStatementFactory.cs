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
    public class RDomReturnStatementFactory
                : RDomStatementFactory<RDomReturnStatement, ReturnStatementSyntax>
    {
        protected  override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent,SemanticModel model)
        {
            var syntax = syntaxNode as ReturnStatementSyntax;
            var newItem = new RDomReturnStatement(syntaxNode,parent, model);

            if (syntax.Expression != null)
            {
                newItem.Return = RDomFactoryHelper.GetHelperForExpression().MakeItems(syntax.Expression, newItem, model).FirstOrDefault();
                if (newItem.Return == null) throw new InvalidOperationException();
            }

            return newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatementCommentWhite item)
        {
            var itemAsT = item as IReturnStatement;
            var node = SyntaxFactory.ReturnStatement();
            if (itemAsT.Return != null)
            {
                var returnExpressionSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Return);
                node = node.WithExpression((ExpressionSyntax)returnExpressionSyntax);
            }

            return item.PrepareForBuildSyntaxOutput(node);
        }
    }
}
