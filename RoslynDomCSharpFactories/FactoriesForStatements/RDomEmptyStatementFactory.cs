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
    public class RDomEmptyStatementFactory
                : RDomStatementFactory<RDomEmptyStatement, EmptyStatementSyntax>
    {
        public RDomEmptyStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent,SemanticModel model)
        {
            var syntax = syntaxNode as EmptyStatementSyntax;
            var newItem = new RDomEmptyStatement(syntaxNode,parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            return newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IEmptyStatement;
            var node = SyntaxFactory.EmptyStatement();

            return node.PrepareForBuildSyntaxOutput(item);
        }
    }
}
