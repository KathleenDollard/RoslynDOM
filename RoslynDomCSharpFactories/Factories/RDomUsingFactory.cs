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
        public override IEnumerable<IStemMember> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as UsingDirectiveSyntax;
            var newItem = new RDomUsing(syntaxNode,parent, model);

            newItem. Name = syntax.Name.NameFrom();

            return new IStemMember[] { newItem };
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
