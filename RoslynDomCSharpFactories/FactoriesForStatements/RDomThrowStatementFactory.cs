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
    public class RDomThrowStatementFactory
                : RDomStatementFactory<RDomThrowStatement, ThrowStatementSyntax>
    {
        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ThrowStatementSyntax;
            var newItem = new RDomThrowStatement(syntaxNode, parent, model);

            if (syntax.Expression != null)
            {
                var expression =   RDomFactoryHelper.GetHelperForExpression().MakeItems(syntax.Expression, newItem, model).FirstOrDefault();
                newItem.ExceptionExpression = expression;
            }

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatementCommentWhite item)
        {
            var itemAsT = item as IThrowStatement;
            var node = SyntaxFactory.ThrowStatement();
            var exception = (ExpressionSyntax)RDomCSharpFactory.Factory.BuildSyntax(itemAsT.ExceptionExpression);
            if (exception != null) node = node.WithExpression(exception);
            return item.PrepareForBuildSyntaxOutput(node);
        }
    }
}
