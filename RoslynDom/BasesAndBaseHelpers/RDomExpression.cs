using System;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomExpression : RDomBase<IExpression, ISymbol>, IExpression
    {
        public RDomExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomExpression(RDomExpression oldRDom)
            : base(oldRDom)
        {
            Expression = oldRDom.Expression;
            ExpressionType = oldRDom.ExpressionType;
        }

        public string Expression { get; set; }

        public ExpressionType ExpressionType { get; set; }
    
    }
}
