using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    //public class RDomUsingStemMemberFactory
    //        : RDomStemMemberFactory<RDomUsing, UsingDirectiveSyntax>
    //{ }
    
    public class RDomUsing : RDomBase<IUsing, UsingDirectiveSyntax, ISymbol>, IUsing
    {
        internal RDomUsing(
            UsingDirectiveSyntax rawItem)
           : base(rawItem)
        {
            Initialize2();
        }

        internal RDomUsing(
            UsingDirectiveSyntax rawItem,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        {
            Initialize();
        }

        internal RDomUsing(RDomUsing oldRDom)
             : base(oldRDom)
        { }

        protected override void Initialize()
        {
            base.Initialize();
            Name = TypedSyntax.Name.NameFrom();
        }

        protected void Initialize2()
        {
            Initialize();
        }

        public override UsingDirectiveSyntax BuildSyntax()
        {
            var nameSyntax = SyntaxFactory.IdentifierName(Name);
            var node = SyntaxFactory.UsingDirective(nameSyntax);

            return (UsingDirectiveSyntax)RoslynUtilities.Format(node);
        }

        public StemMemberKind StemMemberKind
        { get { return StemMemberKind.Using; } }
    }
}
