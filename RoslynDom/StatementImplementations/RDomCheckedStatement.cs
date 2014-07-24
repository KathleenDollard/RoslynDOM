using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomCheckedStatement : RDomBase<ICheckedStatement, ISymbol>, ICheckedStatement
    {
        private RDomList<IStatementCommentWhite> _statements;

        public RDomCheckedStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomCheckedStatement(RDomCheckedStatement oldRDom)
            : base(oldRDom)
        {
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            StatementsAll.AddOrMoveRange(statements);
            HasBlock = oldRDom.HasBlock;
            Unchecked  = oldRDom.Unchecked;
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

        public IEnumerable<IStatement> Statements
        { get { return _statements.OfType<IStatement>().ToList(); } }
         
        public RDomList<IStatementCommentWhite> StatementsAll
        { get { return _statements; } }

        public bool Unchecked { get; set; }
    }
}
