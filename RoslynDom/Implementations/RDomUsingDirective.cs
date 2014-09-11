using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomUsingDirective : RDomBase<IUsingDirective, ISymbol>, IUsingDirective
    {
        public RDomUsingDirective(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { }

        internal RDomUsingDirective(RDomUsingDirective oldRDom)
            : base(oldRDom)
        {
            Alias = oldRDom.Alias;
        }

        public string Name { get; set; }

        public string Alias { get; set; }

        //public string OuterName
        //{ get { return RoslynUtilities.GetOuterName(this); } }

        public StemMemberKind StemMemberKind
        { get { return StemMemberKind.UsingDirective; } }
    }
}
