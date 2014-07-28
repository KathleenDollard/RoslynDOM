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
        public RDomWhileStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as WhileStatementSyntax;
            var newItem = new RDomWhileStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            newItem.TestAtEnd = false; // restating the obvious
            return LoopFactoryHelper.CreateItemFrom<IWhileStatement>(newItem, syntax.Condition, syntax.Statement, parent, model, Corporation, CreateFromWorker);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IWhileStatement;
            return LoopFactoryHelper.BuildSyntax<IWhileStatement>(itemAsT, (c, s) => SyntaxFactory.WhileStatement(c, s));
        }


    }
}
