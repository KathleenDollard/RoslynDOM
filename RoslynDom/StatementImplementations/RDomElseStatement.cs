using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomElseStatement : RDomIfBaseStatement<IFinalElseStatement>, IFinalElseStatement
    {
        public RDomElseStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

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
