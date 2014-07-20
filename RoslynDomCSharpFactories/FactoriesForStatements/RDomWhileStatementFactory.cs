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
         : RDomStatementFactory<RDomWhileStatement, WhileStatementSyntax>
    {
        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as WhileStatementSyntax;
            var newItem = new RDomWhileStatement(syntaxNode, parent, model);
            newItem.TestAtEnd = false; // restating the obvious
            return LoopFactoryHelper.CreateFrom<IWhileStatement>(newItem, syntax.Condition, syntax.Statement, parent, model);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IWhileStatement;
            return LoopFactoryHelper.BuildSyntax<IWhileStatement>(itemAsT, (c, s) => SyntaxFactory.WhileStatement(c, s));
        }


    }
}
