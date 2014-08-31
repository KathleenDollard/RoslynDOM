using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomInvocationStatement : RDomBase<IInvocationStatement, ISymbol>, IInvocationStatement
    {

        public RDomInvocationStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
           "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomInvocationStatement(RDomInvocationStatement oldRDom)
           : base(oldRDom)
        {
            Invocation = oldRDom.Invocation.Copy();
        }

        public override IEnumerable<IDom> Children
        { get { return new List<IDom>() { Invocation }; } }

        public override IEnumerable<IDom> Descendants
        { get { return new List<IDom>() { Invocation }; } }

        public IExpression Invocation { get; set; }
    }
}
