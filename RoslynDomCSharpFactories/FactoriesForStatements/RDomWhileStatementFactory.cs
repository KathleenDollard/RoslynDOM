using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomWhileStatementFactory
         : RDomBaseLoopStatementFactory<RDomWhileStatement, WhileStatementSyntax>
    {
        public RDomWhileStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override ExpressionSyntax GetConditionFromSyntax(WhileStatementSyntax syntax)
        {
            return syntax.Condition;
        }

        protected override StatementSyntax GetStatementFromSyntax(WhileStatementSyntax syntax)
        {
            return syntax.Statement;
        }

        protected override RDomWhileStatement MakeNewItem(WhileStatementSyntax syntax, IDom parent, SemanticModel model)
        {
            return new RDomWhileStatement(syntax, parent, model);
        }

        protected override WhileStatementSyntax MakeSyntax(
                RDomWhileStatement itemAsT, ExpressionSyntax condition, StatementSyntax statementBlock)
        {
            return SyntaxFactory.WhileStatement( condition,statementBlock);
        }
    }
}
