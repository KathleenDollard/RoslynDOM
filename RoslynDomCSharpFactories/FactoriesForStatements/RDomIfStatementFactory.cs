using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomIfStatementFactory
         : RDomStatementFactory<RDomIfStatement, IfStatementSyntax>
    {
        public override IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as IfStatementSyntax;
            var newItem = new RDomIfStatement(syntaxNode, parent, model);
            newItem.Condition = GetCondition(newItem, syntax.Condition, model);

            InitializeStatements(newItem,  syntax.Statement, model);
            var elseIfSyntaxList = GetElseIfSyntaxList(syntax);
            foreach (var elseIf in elseIfSyntaxList.Skip(1))  // The first is the root if
            {
                var newElse = new RDomElseIfStatement(elseIf, newItem, model);
                newElse.Condition = GetCondition(newElse, elseIf.Condition, model);
                InitializeStatements(newElse, elseIf.Statement, model);
                newItem.AddOrMoveElse(newElse);
            }
            var lastElseIf = elseIfSyntaxList.Last();
            if (lastElseIf.Else != null && lastElseIf.Else.Statement != null)
            {
                var newElse = new RDomElseStatement(syntax, newItem, model);
                InitializeStatements(newElse,  lastElseIf.Else.Statement, model);
                newItem.AddOrMoveElse(newElse);
            }
            return new IStatement[] { newItem };
        }

        public IExpression GetCondition(IIfBaseStatement newItem, ExpressionSyntax condition, SemanticModel model)
        {
            if (condition == null) { return null; }
            return RDomFactoryHelper.GetHelper<IExpression>().MakeItem(condition, newItem, model).FirstOrDefault();
        }

        public void InitializeStatements(IStatementContainer  newItem,  StatementSyntax statementSytax, SemanticModel model)
        {
            bool hasBlock = false;
            var statements = RoslynCSharpUtilities.GetStatementsFromSyntax(statementSytax, newItem, ref hasBlock, model);
            newItem.HasBlock = hasBlock;
            foreach (var statement in statements)
            { newItem.AddOrMoveStatement(statement); }
        }

        private IEnumerable<IfStatementSyntax> GetElseIfSyntaxList(IfStatementSyntax syntax)
        {
            // You can't use descendants here becuase it is a very specific pattern 
            var list = new List<IfStatementSyntax>();
            list.Add(syntax);
            if (syntax.Else != null)
            {
                var elseAsIf = syntax.Else.Statement as IfStatementSyntax;
                if (elseAsIf == null) { } // At terminus of recursion
                else
                {
                    // Recurse down the if chain
                    list.AddRange(GetElseIfSyntaxList(elseAsIf));
                }
            }
            return list;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IIfStatement;
            var elseSyntax = BuildElseSyntax(itemAsT.Elses);
            var node = SyntaxFactory.IfStatement(GetCondition(itemAsT), GetStatement(itemAsT));
            if (elseSyntax != null) { node = node.WithElse(elseSyntax); }

            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }

        private ElseClauseSyntax BuildElseSyntax(IEnumerable<IElseStatement> elses)
        {
            // Because we reversed the list, inner is first, inner to outer required for this approach
            elses = elses.Reverse();
            ElseClauseSyntax elseClause = null;
            foreach (var nestedElse in elses)
            {
                var statement = GetStatement(nestedElse);
                var elseIf = nestedElse as IElseIfStatement;
                if (elseIf != null)
                {
                    // build if statement and put in else clause
                    statement = SyntaxFactory.IfStatement(GetCondition(elseIf), statement)
                                .WithElse(elseClause);
                }
                var newElseClause = SyntaxFactory.ElseClause(statement);
                elseClause = newElseClause;
            }
            return elseClause;
        }

        private static ExpressionSyntax GetCondition(IHasCondition itemAsT)
        { return (ExpressionSyntax)RDomCSharpFactory.Factory.BuildSyntax(itemAsT.Condition); }

        private static StatementSyntax GetStatement(IStatementContainer itemAsT)
        { return RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT.HasBlock); }
    }
}
