using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomIfStatementFactory
         : RDomStatementFactory<RDomIfStatement, IfStatementSyntax>
    {
        public override void InitializeItem(RDomIfStatement newItem, IfStatementSyntax syntax)
        {
            newItem.Condition = RDomFactoryHelper.GetHelper<IExpression>().MakeItem(syntax.Condition).FirstOrDefault();
            if (syntax.Condition == null) { throw new InvalidOperationException(); }
            bool hasBlock = false;
            var statements = GetStatementsFromSyntax(syntax.Statement, ref hasBlock);
            newItem.HasBlock  = hasBlock;
            foreach (var statement in statements)
            { newItem.AddOrMoveStatement(statement); }

            InitializeElse(newItem, syntax);
        }

        private void InitializeElse(RDomIfStatement newItem, IfStatementSyntax syntax)
        {
            if (syntax.Else == null) return;
            var elseAsIf = syntax.Else.Statement as IfStatementSyntax;
            if (elseAsIf == null)
            {
                InitializeElseStatement(newItem, syntax,syntax.Else.Statement);
            }
            else
            {
                // Recurse this down the if chain
                var newIf = new RDomIfStatement(elseAsIf);
                InitializeItem(newIf, elseAsIf);
                var elseIfs = newIf.ElseIfs;
                var elseStatements = newIf.ElseStatements;
                //foreach (var item in newIf.ElseIfs) { newIf.RemoveElseIf(item); }
                //foreach (var item in newIf.ElseStatements) { newIf.RemoveElseStatement(item); }
                newItem. AddOrMoveElseIf(newIf);
                if (!newIf.ElseIfs.Any())
                {
                    // this should move them
                    foreach (var elseif in elseIfs)
                    { newItem.AddOrMoveElseIf(elseif); }  // Don't need to copy as we are trashing original
                    if (elseStatements.Any())
                    {
                        newItem.ElseHasBlock = newIf.ElseHasBlock;
                        foreach (var statement in elseStatements)
                        { newItem.AddOrMoveElseStatement(statement); }
                    }
                }
            }
        }

        private void InitializeElseStatement(RDomIfStatement newItem, IfStatementSyntax syntax,StatementSyntax statement)
        {
            newItem.ElseHasBlock = statement is BlockSyntax;
            bool hasBlock = false;
            var statements = GetStatementsFromSyntax(syntax.Else.Statement, ref hasBlock);
            newItem.ElseHasBlock = hasBlock;
            foreach (var state in statements)
            { newItem.AddOrMoveElseStatement(state); }
        }

        private IEnumerable<IStatement> GetStatementsFromSyntax(StatementSyntax statementSyntax, ref bool hasBlock)
        {
            var statement = RDomFactoryHelper.GetHelper<IStatement>().MakeItem(statementSyntax).First();
            var list = new List<IStatement>();
            var blockStatement = statement as IBlockStatement;
            if (blockStatement != null)
            {
                hasBlock = true;
                foreach (var state in blockStatement.Statements)
                {
                    // Don't need to copy because abandoning block
                    list.Add(state);
                }
            }
            else
            { list.Add(statement); }
            return list;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var node = BuildSyntax(item, true);
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }

        private SyntaxNode BuildSyntax(IStatement item, bool isIfRoot)
        {
            var itemAsT = item as IIfStatement;
            var elseSyntax = BuildElseSyntax(itemAsT.ElseIfs, itemAsT.ElseStatements, itemAsT.ElseHasBlock);

            return BuildSyntax(item, isIfRoot, elseSyntax);
        }

        private SyntaxNode BuildSyntax(IStatement item, bool isIfRoot, ElseClauseSyntax elseClauseSyntax)
        {
            var itemAsT = item as IIfStatement;

            if (itemAsT.Condition == null) return SyntaxFactory.EmptyStatement(); // This shold not happen 

            var statement = BuildStatement(itemAsT.Statements, itemAsT.HasBlock);
            var condition = RDomFactory.BuildSyntax(itemAsT.Condition);
            var node = SyntaxFactory.IfStatement((ExpressionSyntax)condition, statement);

            if (elseClauseSyntax != null) { node = node.WithElse(elseClauseSyntax); }

            return node;
        }

        private StatementSyntax BuildStatement(IEnumerable<IStatement> statements, bool hasBlock)
        {
            StatementSyntax statement;
            var statementSyntaxList = statements
                         .SelectMany(x => RDomFactory.BuildSyntaxGroup(x))
                         .ToList();
            if (hasBlock || statements.Count() > 1)
            { statement = SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList)); }
            else if (statements.Count() == 1)
            { statement = (StatementSyntax)statementSyntaxList.First(); }
            else
            { statement = SyntaxFactory.EmptyStatement(); }
            return statement;
        }

        private ElseClauseSyntax BuildElseSyntax(IEnumerable<IIfStatement> elseIfs, IEnumerable<IStatement> elseStatements, bool elseHasBlock)
        {
            // Because we reversed the list, inner is first, inner to outer required for this approach
            elseIfs = elseIfs.Reverse();
            ElseClauseSyntax elseClause = null;
            if (elseStatements.Any())
            { elseClause = SyntaxFactory.ElseClause(BuildStatement(elseStatements, elseHasBlock)); }
            foreach (var nestedIf in elseIfs)
            {
                var statement = BuildStatement(nestedIf.Statements, nestedIf.HasBlock);
                // No need to go back to factory, as it would just return us here
                ElseClauseSyntax newElseClause;

                var newIfClause = BuildSyntax(nestedIf, false, elseClause);
                newElseClause = SyntaxFactory.ElseClause((StatementSyntax)newIfClause);
                elseClause = newElseClause;
            }
            return elseClause;
        }
    }
}
