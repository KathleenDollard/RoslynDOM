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
    public class RDomContinueStatementFactory
                : RDomStatementFactory<RDomContinueStatement, ContinueStatementSyntax>
    {
        public RDomContinueStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ContinueStatementSyntax;
            var newItem = new RDomContinueStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IContinueStatement;
            var node = SyntaxFactory.ContinueStatement();

            return item.PrepareForBuildSyntaxOutput(node);
        }
    }
}
