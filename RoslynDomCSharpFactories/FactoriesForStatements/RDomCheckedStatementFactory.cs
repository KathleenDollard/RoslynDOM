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
    public class RDomCheckedStatementFactory
                : RDomStatementFactory<RDomCheckedStatement, CheckedStatementSyntax>
    {
        public RDomCheckedStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as CheckedStatementSyntax;
            var newItem = new RDomCheckedStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            newItem.Unchecked = (syntax.CSharpKind() == SyntaxKind.UncheckedStatement);

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as ICheckedStatement;
            var statement = RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT.HasBlock) as BlockSyntax;
            var kind = itemAsT.Unchecked ? SyntaxKind.UncheckedExpression : SyntaxKind.CheckedExpression;

            var node = SyntaxFactory.CheckedStatement(kind, statement);


            return itemAsT.PrepareForBuildSyntaxOutput(node);
        }
    }
}
