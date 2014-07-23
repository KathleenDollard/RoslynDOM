using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomUsingStemMemberFactory
            : RDomStemMemberFactory<RDomUsing, UsingDirectiveSyntax>
    {
        protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as UsingDirectiveSyntax;
            var newItem = new RDomUsing(syntaxNode,parent, model);

            newItem. Name = syntax.Name.NameFrom();

            return  newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMemberCommentWhite item)
        {
            // TODO: Handle alias's
            // TODO: Handle using statements, that's not done (the other usings)
            var itemAsT = item as IUsing;
            var identifier = SyntaxFactory.IdentifierName(itemAsT.Name);
            var node = SyntaxFactory.UsingDirective(identifier);

            // TODO: return new SyntaxNode[] { node.Format() };
            return new SyntaxNode[] { node };
        }
    }

}
