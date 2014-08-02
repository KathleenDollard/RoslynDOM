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
        public RDomUsingDirectiveStemMemberFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as UsingDirectiveSyntax;
            var newItem = new RDomUsingDirective(syntaxNode,parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            newItem. Name = syntax.Name.NameFrom();
            if (syntax.Alias != null)
            { newItem.Alias = syntax.Alias.ToString().Replace("=", "").Trim(); }

            return  newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            // TODO: Handle alias's
            // TODO: Handle using statements, that's not done (the other usings)
            var itemAsT = item as IUsingDirective;
            var identifier = SyntaxFactory.IdentifierName(itemAsT.Name);
            var node = SyntaxFactory.UsingDirective(identifier);
            if (!string.IsNullOrWhiteSpace(itemAsT.Alias))
            { node = node.WithAlias(SyntaxFactory.NameEquals(itemAsT.Alias)); }
            return item.PrepareForBuildSyntaxOutput(node);
        }
    }

}
