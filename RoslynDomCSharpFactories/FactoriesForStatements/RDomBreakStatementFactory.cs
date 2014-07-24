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
    public class RDomBreakStatementFactory
                : RDomStatementFactory<RDomBreakStatement, BreakStatementSyntax>
    {
        protected  override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent,SemanticModel model)
        {
            var syntax = syntaxNode as BreakStatementSyntax;
            var newItem = new RDomBreakStatement(syntaxNode,parent, model);

            return newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatementCommentWhite item)
        {
            var itemAsT = item as IBreakStatement;
            var node = SyntaxFactory.BreakStatement();

            return item.PrepareForBuildSyntaxOutput(node);
        }
    }
}
