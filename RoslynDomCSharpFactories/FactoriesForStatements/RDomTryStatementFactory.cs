﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomTryStatementFactory
         : RDomStatementFactory<RDomTryStatement, TryStatementSyntax>
    {
        public RDomTryStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as TryStatementSyntax;
            var newItem = new RDomTryStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            var catchSyntaxList = syntax.ChildNodes()
                                    .Where(x => x.CSharpKind() == SyntaxKind.CatchClause)
                                    .OfType<CatchClauseSyntax>();
            foreach (var ctch in catchSyntaxList)  // The first is the root if
            {
                var newCatch = new RDomCatchStatement(ctch, newItem, model);
                CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
                var newVariable = Corporation
                            .CreateFrom<IMisc>(ctch.Declaration, newCatch, model)
                            .OfType<IVariableDeclaration>()
                            .FirstOrDefault();
                newCatch.ExceptionVariable = newVariable;
                newCatch.Condition = Corporation.CreateFrom<IExpression>(ctch.Filter.FilterExpression, newCatch, model).FirstOrDefault();
                newItem.CatchesAll.AddOrMove(newCatch);
            }
            var newFinally = new RDomFinallyStatement(syntax.Finally, newItem, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            newItem.Finally = newFinally ;
            return newItem ;
        }

      

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IIfStatement;
            var elseSyntax = BuildElseSyntax(itemAsT.Elses);
            var node = SyntaxFactory.IfStatement(BuildSyntaxHelpers.GetCondition(itemAsT), BuildSyntaxHelpers.GetStatement(itemAsT));
            if (elseSyntax != null) { node = node.WithElse(elseSyntax); }

            return item.PrepareForBuildSyntaxOutput(node);
        }

        private ElseClauseSyntax BuildElseSyntax(IEnumerable<IElseStatement> elses)
        {
            // Because we reversed the list, inner is first, inner to outer required for this approach
            elses = elses.Reverse();
            ElseClauseSyntax elseClause = null;
            foreach (var nestedElse in elses)
            {
                var statement = BuildSyntaxHelpers.GetStatement(nestedElse);
                var elseIf = nestedElse as IElseIfStatement;
                if (elseIf != null)
                {
                    // build if statement and put in else clause
                    statement = SyntaxFactory.IfStatement(BuildSyntaxHelpers.GetCondition(elseIf), statement)
                                .WithElse(elseClause);
                }
                var newElseClause = SyntaxFactory.ElseClause(statement);
                elseClause = newElseClause;
            }
            return elseClause;
        }

   
    }
}