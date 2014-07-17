using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomIfStatement : RDomBase<IIfStatement, ISymbol>, IIfStatement
    {
        private IList<IIfStatement> _elses = new List<IIfStatement>();
        private IList<IStatement> _statements = new List<IStatement>();
        private IList<IStatement> _elseStatements = new List<IStatement>();

        public RDomIfStatement(SyntaxNode rawItem, SemanticModel model)
           : base(rawItem, model)
        { }

        internal RDomIfStatement(RDomIfStatement oldRDom)
            : base(oldRDom)
        {
            var newElses = RoslynDomUtilities.CopyMembers(oldRDom.ElseIfs);
            foreach (var elseItem in newElses)
            { AddOrMoveElseIf(elseItem); }
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            foreach (var statement in statements)
            { AddOrMoveStatement(statement); }
            statements = RoslynDomUtilities.CopyMembers(oldRDom.ElseStatements);
            foreach (var statement in statements)
            { AddOrMoveElseStatement(statement); }
            Condition = oldRDom.Condition.Copy();
            HasBlock = oldRDom.HasBlock;
            ElseHasBlock = oldRDom.ElseHasBlock;
        }

        public IExpression Condition { get; set; }

        public bool HasBlock { get; set; }
        public bool ElseHasBlock { get; set; }

        public void RemoveElseIf(IIfStatement statement)
        { _elses.Remove(statement); }

        public void AddOrMoveElseIf(IIfStatement statement)
        { _elses.Add(statement); }

        public IEnumerable<IIfStatement> ElseIfs
        { get { return _elses; } }


        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddOrMoveStatement(IStatement statement)
        { _statements.Add(statement); }

        public IEnumerable<IStatement> Statements
        { get { return _statements; } }

        public void RemoveElseStatement(IStatement statement)
        { _elseStatements.Remove(statement); }

        public void AddOrMoveElseStatement(IStatement statement)
        { _elseStatements.Add(statement); }

        public IEnumerable<IStatement> ElseStatements
        { get { return _elseStatements; } }
    }
}
