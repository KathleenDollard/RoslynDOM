using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public abstract class RDomBaseLoop<T>
        : RDomBase<T, ISymbol>, IStatement
        where T : class, ILoop<T>, IStatement
    {
        private IList<IStatement> _statements = new List<IStatement>();

        internal RDomBaseLoop(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        {  }


        internal RDomBaseLoop(T oldRDom)
             : base(oldRDom)
        {
            foreach (var statement in Statements)
            { AddOrMoveStatement(statement); }
            Condition = oldRDom.Condition.Copy();
            HasBlock = oldRDom.HasBlock;
            TestAtEnd = oldRDom.TestAtEnd;
        }

        public IExpression Condition { get; set; }

        public bool HasBlock { get; set; }
        public bool TestAtEnd { get; set; }

        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddOrMoveStatement(IStatement statement)
        { _statements.Add(statement); }

        public IEnumerable<IStatement> Statements
        { get { return _statements; } }

    }
}

