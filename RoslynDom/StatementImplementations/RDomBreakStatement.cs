using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomBreakStatement : RDomBase<IBreakStatement, ISymbol>, IBreakStatement
    {

        public RDomBreakStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomBreakStatement(RDomBreakStatement oldRDom)
            : base(oldRDom)
        { }
    }
}
