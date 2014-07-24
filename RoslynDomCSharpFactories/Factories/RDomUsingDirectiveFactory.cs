using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomUsingDirectiveStemMemberFactory
            : RDomStemMemberFactory<RDomUsingDirective, UsingDirectiveSyntax>
    {
        protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as UsingDirectiveSyntax;
            var newItem = new RDomUsingDirective(syntaxNode,parent, model);

            newItem. Name = syntax.Name.NameFrom();

            return  newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMemberCommentWhite item)
        {
            // TODO: Handle alias's
            // TODO: Handle using statements, that's not done (the other usings)
            var itemAsT = item as IUsingDirective;
            var identifier = SyntaxFactory.IdentifierName(itemAsT.Name);
            var node = SyntaxFactory.UsingDirective(identifier);

            return item.PrepareForBuildSyntaxOutput(node);
        }
    }

}
