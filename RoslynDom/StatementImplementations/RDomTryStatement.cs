using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
 using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
    public class RDomTryStatement : RDomStatementBlockBase <ITryStatement>, ITryStatement
    {
        private RDomCollection<ICatchStatement> _catches;

        public RDomTryStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomTryStatement(RDomTryStatement oldRDom)
            : base(oldRDom)
        {
            Initialize();
            // Initialize called in base
            var newCatches = RoslynDomUtilities.CopyMembers(oldRDom.Catches);
            CatchesAll.AddOrMoveRange(newCatches);
            Finally = oldRDom.Finally.Copy();
        }

        private void Initialize()
        {
            _catches = new RDomCollection<ICatchStatement>(this);
        }

        public override IEnumerable<IDom> Children
        {
            get
            {
                var list = new List<IDom>();
                list.AddRange(base.Children.ToList());
                list.AddRange(Catches);
                list.Add(Finally);
                return list;
            }
        }

        public RDomCollection<ICatchStatement> CatchesAll
        { get { return _catches; } }

        public IEnumerable<ICatchStatement> Catches
        { get { return _catches; } }

        public IFinallyStatement Finally { get; set; }
    }
}
