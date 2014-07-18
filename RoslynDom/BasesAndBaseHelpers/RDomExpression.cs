using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomExpression : RDomBase<IExpression, ISymbol>, IExpression
    {
        internal RDomExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomExpression(RDomExpression oldRDom)
            : base(oldRDom)
        {
            Expression = oldRDom.Expression;
        }

        public string Expression { get; set; }
    }
}
