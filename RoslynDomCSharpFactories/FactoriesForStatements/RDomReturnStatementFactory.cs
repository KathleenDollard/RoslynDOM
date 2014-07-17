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
        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode, SemanticModel model)
        {
            var syntax = syntaxNode as ReturnStatementSyntax;
            var newItem = new RDomReturnStatement(syntaxNode, model);

            if (syntax.Expression != null)
            {
                newItem.Return = RDomFactoryHelper.GetHelper<IExpression>().MakeItem(syntax.Expression, model).FirstOrDefault();
                if (newItem.Return == null) throw new InvalidOperationException();
            }

            return new IStatement[] { newItem };
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IReturnStatement;
            var node = SyntaxFactory.ReturnStatement();
            if (itemAsT.Return != null)
            {
                var returnExpressionSyntax = RDomCSharpFactory.Factory.BuildSyntax(itemAsT.Return);
                node = node.WithExpression((ExpressionSyntax)returnExpressionSyntax);
            }
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }
}
