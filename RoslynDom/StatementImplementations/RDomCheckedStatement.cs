using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
 using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
    public class RDomCheckedStatement : RDomBase<ICheckedStatement, ISymbol>, ICheckedStatement
    {
        private RDomCollection<IStatementCommentWhite> _statements;

        public RDomCheckedStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomCheckedStatement(RDomCheckedStatement oldRDom)
            : base(oldRDom)
        {
            Initialize();
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            StatementsAll.AddOrMoveRange(statements);
            HasBlock = oldRDom.HasBlock;
            Unchecked  = oldRDom.Unchecked;
        }

        protected  void Initialize()
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

        private bool _unchecked ;
        public bool Unchecked { get {return _unchecked; }
set {SetProperty(ref _unchecked, value); }}
        private bool _hasBlock ;
        public bool HasBlock { get {return _hasBlock; }
set {SetProperty(ref _hasBlock, value); }}

        public IEnumerable<IStatement> Statements
        { get { return _statements.OfType<IStatement>().ToList(); } }

        public RDomCollection<IStatementCommentWhite> StatementsAll
        { get { return _statements; } }
    }
}
