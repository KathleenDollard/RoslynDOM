using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomNamespace : RDomBaseStemContainer<INamespace, INamespaceSymbol>, INamespace
    {
        public RDomNamespace(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomNamespace(RDomNamespace oldRDom)
            : base(oldRDom)
        {
            //_outerName = oldRDom.OuterName;

        }

        public string Name { get; set; }

        public override string OuterName
        { get { return QualifiedName; } }

        public StemMemberKind StemMemberKind
        { get { return StemMemberKind.Namespace; } }
    }
}
