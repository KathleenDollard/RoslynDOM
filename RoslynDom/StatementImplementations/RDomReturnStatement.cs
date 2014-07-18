using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomReturnStatement : RDomBase<IReturnStatement, ISymbol>, IReturnStatement
    {

        public RDomReturnStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomReturnStatement(RDomReturnStatement oldRDom)
            : base(oldRDom)
        {
            Return = oldRDom.Return.Copy();
        }

        public IExpression Return { get; set; }
    }
}
