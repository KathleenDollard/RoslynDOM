using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomStatementBlockBase<T> : RDomBase<T, ISymbol>, IStatementBlock
        where T : class, IStatementBlock, IDom<T>
    {
        private RDomCollection<IStatementCommentWhite> _statements;

        public RDomStatementBlockBase(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        {
            Initialize();
        }

        internal RDomStatementBlockBase(T oldRDom)
            : base(oldRDom)
        {
            Initialize();
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            StatementsAll.AddOrMoveRange(statements);
            HasBlock = oldRDom.HasBlock;
        }

        protected void Initialize()
        {
            _statements = new RDomCollection<IStatementCommentWhite>(this);
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

        public RDomCollection<IStatementCommentWhite> StatementsAll
        { get { return _statements; } }

        public IEnumerable <IStatement> Statements
        { get { return _statements.OfType<IStatement>().ToList(); } }

    }
}
