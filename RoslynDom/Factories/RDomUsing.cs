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

}
