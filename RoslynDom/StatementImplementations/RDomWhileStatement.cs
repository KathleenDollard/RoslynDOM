using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomWhileStatement : RDomBaseLoop<IWhileStatement>, IWhileStatement
    {
        public RDomWhileStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        {
            this.TestAtEnd = true;
            Initialize();
        }

        internal RDomWhileStatement(RDomWhileStatement oldRDom)
            : base(oldRDom)
        { }


        public override IEnumerable<IDom> Children
        { get { return base.Children; } }

        public override IEnumerable<IDom> Descendants
        { get { return base.Descendants; } }
    }
}
