using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomEmptyStatement : RDomBase<IEmptyStatement, ISymbol>, IEmptyStatement
    {

        public RDomEmptyStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomEmptyStatement(RDomEmptyStatement oldRDom)
            : base(oldRDom)
        { }
    }
}
