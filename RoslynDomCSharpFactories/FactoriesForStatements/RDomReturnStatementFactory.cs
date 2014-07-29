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
        public RDomReturnStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent,SemanticModel model)
        {
            var syntax = syntaxNode as ReturnStatementSyntax;
            var newItem = new RDomReturnStatement(syntaxNode,parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            if (syntax.Expression != null)
            {
                newItem.Return = Corporation.CreateFrom<IExpression>(syntax.Expression, newItem, model).FirstOrDefault();
                Guardian.Assert.IsNotNull(newItem.Return, nameof(newItem.Return));
            }

            return newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
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
