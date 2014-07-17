using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomExpression : RDomBase<IExpression, ISymbol>, IExpression
    {
        internal RDomExpression(SyntaxNode rawItem, SemanticModel model)
           : base(rawItem, model)
        { }

        internal RDomExpression(RDomExpression oldRDom)
            : base(oldRDom)
        {
            Expression = oldRDom.Expression;
        }

        public string Expression { get; set; }
    }
}
