using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomUsingStemMemberFactory
            : RDomStemMemberFactory<RDomUsing, UsingDirectiveSyntax>
    {
        public override void InitializeItem(RDomUsing newItem, UsingDirectiveSyntax syntax)
        {
            newItem. Name = syntax.Name.NameFrom();
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {
            // TODO: Handle alias's
            // TODO: Handle using statements, that's not done
            var identifier = SyntaxFactory.IdentifierName(item.Name);
            var node = SyntaxFactory.UsingDirective(identifier);
            
            return new SyntaxNode[] { node.NormalizeWhitespace() };
        }
    }

    public class RDomUsing : RDomBase<IUsing, UsingDirectiveSyntax, ISymbol>, IUsing
    {
        internal RDomUsing(
            UsingDirectiveSyntax rawItem)
           : base(rawItem)
        {
            //Initialize2();
        }

        //internal RDomUsing(
        //    UsingDirectiveSyntax rawItem,
        //    params PublicAnnotation[] publicAnnotations)
        //  : base(rawItem, publicAnnotations)
        //{
        //    Initialize();
        //}

        internal RDomUsing(RDomUsing oldRDom)
             : base(oldRDom)
        { }

        //protected override void Initialize()
        //{
        //    base.Initialize();
        //    Name = TypedSyntax.Name.NameFrom();
        //}

        //protected void Initialize2()
        //{
        //    Initialize();
        //}

        //public override UsingDirectiveSyntax BuildSyntax()
        //{
        //    var nameSyntax = SyntaxFactory.IdentifierName(Name);
        //    var node = SyntaxFactory.UsingDirective(nameSyntax);

        //    return (UsingDirectiveSyntax)RoslynUtilities.Format(node);
        //}

        public StemMemberKind StemMemberKind
        { get { return StemMemberKind.Using; } }
    }
}
