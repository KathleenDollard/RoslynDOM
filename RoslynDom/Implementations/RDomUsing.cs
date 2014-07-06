using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomUsingDirective : RDomBase<IUsing, UsingDirectiveSyntax, ISymbol>, IUsing
    {
        internal RDomUsingDirective(
            UsingDirectiveSyntax rawItem,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        {
            Initialize();
        }

        internal RDomUsingDirective(RDomUsingDirective oldRDom)
             : base(oldRDom)
        { }

        protected override void Initialize()
        {
            base.Initialize();
           Name = TypedSyntax.Name.NameFrom(); 
        }

        public StemMemberType StemMemberType
        { get { return StemMemberType.Using; } }
    }
}
