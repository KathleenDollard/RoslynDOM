using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomIfBaseStatement<T> : RDomBase<T, ISymbol>, IIfBaseStatement
        where T : class, IIfBaseStatement, IDom<T>
    {
        private IList<IStatement> _statements = new List<IStatement>();

        public RDomIfBaseStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomIfBaseStatement(T oldRDom)
            : base(oldRDom)
        {
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            foreach (var statement in statements)
            { AddOrMoveStatement(statement); }
            HasBlock = oldRDom.HasBlock;
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = base.Children.ToList();
                list.AddRange(Statements);
                return list;
            }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            {
                var list = base.Descendants.ToList();
                foreach (var statement in Statements)
                { list.AddRange(statement.DescendantsAndSelf); }
                return list;
            }
        }

        public bool HasBlock { get; set; }

        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddOrMoveStatement(IStatement statement)
        { _statements.Add(statement); }

        public IEnumerable<IStatement> Statements
        { get { return _statements; } }

    }
}
