using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomFinallyStatement : RDomStatementBlockBase<IFinallyStatement>, IFinallyStatement
    {
        public RDomFinallyStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomFinallyStatement(RDomFinallyStatement oldRDom)
            : base(oldRDom)
        { }

        public override IEnumerable<IDom> Children
        {
            get
            { return base.Children.ToList(); }
        }

        //public override IEnumerable<IDom> Descendants
        //{
        //    get
        //    { return base.Descendants.ToList(); }
        //}

    }
}
