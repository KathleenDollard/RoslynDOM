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
    public class RDomLockStatementFactory
                : RDomStatementFactory<RDomLockStatement, LockStatementSyntax>
    {
        public RDomLockStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as LockStatementSyntax;
            var newItem = new RDomLockStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            var expr = Corporation.CreateFrom<IExpression>(syntax.Expression, newItem, model).FirstOrDefault();
            newItem.Expression = expr;

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as ILockStatement;
            var statement = RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT.HasBlock);
            var expressionSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Expression) as ExpressionSyntax;
            var node = SyntaxFactory.LockStatement(expressionSyntax, statement);

            return itemAsT.PrepareForBuildSyntaxOutput(node);
        }
    }
}
