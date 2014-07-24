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
        protected  override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent,SemanticModel model)
        {
            var syntax = syntaxNode as EmptyStatementSyntax;
            var newItem = new RDomEmptyStatement(syntaxNode,parent, model);

            return newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatementCommentWhite item)
        {
            var itemAsT = item as IEmptyStatement;
            var node = SyntaxFactory.EmptyStatement();
          
            return item.PrepareForBuildSyntaxOutput(node);
        }
    }
}
