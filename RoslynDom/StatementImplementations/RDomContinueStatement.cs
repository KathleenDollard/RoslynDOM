using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomContinueStatement : RDomBase<IContinueStatement, ISymbol>, IContinueStatement
    {

        public RDomContinueStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomContinueStatement(RDomContinueStatement oldRDom)
            : base(oldRDom)
        { }
    }
}
