using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomTryStatement : RDomStatementBlockBase <ITryStatement>, ITryStatement
    {
        private RDomList<ICatchStatement> _catches;

        public RDomTryStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomTryStatement(RDomTryStatement oldRDom)
            : base(oldRDom)
        {
            // Initialize called in base
            var newCatches = RoslynDomUtilities.CopyMembers(oldRDom.Catches);
            CatchesAll.AddOrMoveRange(newCatches);
            Finally = oldRDom.Finally;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _catches = new RDomList<ICatchStatement>(this);
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = new List<IDom>();
                list.Add(Finally);
                list.AddRange(base.Children.ToList());
                list.AddRange(Catches);
                return list;
            }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            {
                var list = new List<IDom>();
                list.AddRange(Finally.DescendantsAndSelf);
                list.AddRange(base.Descendants.ToList());
                foreach (var elseif in Catches)
                { list.AddRange(elseif.DescendantsAndSelf); }
                return list;
            }
        }

        public RDomList<ICatchStatement> CatchesAll
        { get { return _catches; } }

        public IEnumerable<ICatchStatement> Catches
        { get { return _catches; } }

        public IFinallyStatement Finally { get; set; }

    }
}
