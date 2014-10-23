using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomDoStatementFactory
         : RDomBaseLoopStatementFactory<RDomDoStatement, DoStatementSyntax>
    {
        public RDomDoStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

         protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var newItem = base.CreateItemFrom(syntaxNode, parent, model) as RDomDoStatement;
            newItem.TestAtEnd = true;
            return newItem;
        }

        protected override ExpressionSyntax GetConditionFromSyntax(DoStatementSyntax syntax)
        {
            return syntax.Condition;
        }

        protected override StatementSyntax GetStatementFromSyntax(DoStatementSyntax syntax)
        {
            return syntax.Statement;
        }

        protected override RDomDoStatement MakeNewItem(DoStatementSyntax syntax, IDom parent, SemanticModel model)
        {
            return new RDomDoStatement(syntax, parent, model);
        }

        protected override DoStatementSyntax MakeSyntax(
                RDomDoStatement itemAsT, ExpressionSyntax condition, StatementSyntax statementBlock)
        {
            return SyntaxFactory.DoStatement(statementBlock, condition);
        }
    }
}
