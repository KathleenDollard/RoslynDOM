using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomUsing : RDomBase<IUsing, ISymbol>, IUsing
    {
        public RDomUsing(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomUsing(RDomUsing oldRDom)
            : base(oldRDom)
        { }

        public StemMemberKind StemMemberKind
        { get { return StemMemberKind.Using; } }
    }
}
