using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomIfBaseStatement<T> : RDomBase<T, ISymbol>, IIfBaseStatement
        where T : class, IIfBaseStatement, IDom<T>
    {
        private RDomList<IStatementCommentWhite> _statements;

        public RDomIfBaseStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomIfBaseStatement(T oldRDom)
            : base(oldRDom)
        {
            Initialize();
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            StatementsAll.AddOrMoveRange(statements);
            HasBlock = oldRDom.HasBlock;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _statements = new RDomList<IStatementCommentWhite>(this);
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

        public RDomList<IStatementCommentWhite> StatementsAll
        { get { return _statements; } }

        public IEnumerable <IStatement> Statements
        { get { return _statements.OfType<IStatement>().ToList(); } }

    }
}
