using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomBlockStatement : RDomBase<IBlockStatement, ISymbol>, IBlockStatement
    {
        private IList<IStatement> _statements = new List<IStatement>();

        public RDomBlockStatement(SyntaxNode rawItem, SemanticModel model)
           : base(rawItem, model)
        { }

        internal RDomBlockStatement(RDomBlockStatement oldRDom)
            : base(oldRDom)
        {
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            foreach (var statement in statements)
            { AddOrMoveStatement(statement); }
        }

        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddOrMoveStatement(IStatement statement)
        { _statements.Add(statement); }

        public IEnumerable<IStatement> Statements
        { get { return _statements; } }
    }
}
