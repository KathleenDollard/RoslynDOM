using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomElseStatement : RDomStatementBlockBase<IFinalElseStatement>, IFinalElseStatement
    {
        public RDomElseStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomElseStatement(RDomElseStatement oldRDom)
            : base(oldRDom)
        { }

        public override IEnumerable<IDom> Children
        {
            get
            { return base.Children.ToList(); }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            { return base.Descendants.ToList(); }
        }

    }
}
