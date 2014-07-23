using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomBlockStatement : RDomBase<IBlockStatement, ISymbol>, IBlockStatement
    {
        private RDomList<IStatementCommentWhite> _statements;

        public RDomBlockStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomBlockStatement(RDomBlockStatement oldRDom)
            : base(oldRDom)
        {
            Initialize();
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            Statements.AddOrMoveRange(statements);
        }

        protected override void Initialize()
        {
            base.Initialize();
            _statements = new RDomList<IStatementCommentWhite>(this);
        }

        public RDomList<IStatementCommentWhite> Statements
        { get { return _statements; } }
    }
}
