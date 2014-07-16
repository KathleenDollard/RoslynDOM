using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharpFactories
{
    public class RDomUsingStemMemberFactory
            : RDomStemMemberFactory<IUsing, UsingDirectiveSyntax>
    {
        public override void InitializeItem(IUsing newItem, UsingDirectiveSyntax syntax)
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
