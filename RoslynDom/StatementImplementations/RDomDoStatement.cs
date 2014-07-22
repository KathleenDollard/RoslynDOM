using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomDoStatement : RDomBaseLoop<IDoStatement>, IDoStatement
    {

        public RDomDoStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        internal RDomDoStatement(RDomDoStatement oldRDom)
            : base(oldRDom)
        { }



        public override IEnumerable<IDom> Children
        {
            get
            { return base.Children; }
        }

        public override IEnumerable<IDom> Descendants
        {
            get
            { return base.Descendants; }
        }

    }
}
