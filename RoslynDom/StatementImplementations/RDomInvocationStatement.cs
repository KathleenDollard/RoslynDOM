using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomInvocationStatement : RDomBase<IInvocationStatement, ISymbol>, IInvocationStatement
    {

        public RDomInvocationStatement(SyntaxNode rawItem, SemanticModel model)
           : base(rawItem, model)
        { }

        internal RDomInvocationStatement(RDomInvocationStatement oldRDom)
           : base(oldRDom)
        {
            Invocation = oldRDom.Invocation.Copy();
        }

        public IExpression Invocation { get; set; }
    }
}
