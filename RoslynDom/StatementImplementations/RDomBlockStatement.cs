using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{
    public class RDomBlockStatement : RDomBase<IBlockStatement, ISymbol>, IBlockStatement
    {
        private RDomCollection<IStatementCommentWhite> _statements;

        public RDomBlockStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomBlockStatement(RDomBlockStatement oldRDom)
            : base(oldRDom)
        {
            Initialize();
            var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
            Statements.AddOrMoveRange(statements);
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
                list.AddRange(_statements);
                return list;
            }
        }

        //public override IEnumerable<IDom> Descendants
        //{
        //    get
        //    {
        //        var list = base.Descendants.ToList();
        //        foreach (var statement in _statements)
        //        { list.AddRange(statement.DescendantsAndSelf); }
        //        return list;
        //    }
        //}

        public RDomCollection<IStatementCommentWhite> Statements
        { get { return _statements; } }
    }
}
