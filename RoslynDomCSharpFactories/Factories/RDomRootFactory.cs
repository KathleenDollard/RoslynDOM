using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomRootFactory
          : RDomRootContainerFactory<RDomRoot, CompilationUnitSyntax>
    {
        protected  override IRoot CreateItemFrom(SyntaxNode syntaxNode, IDom parent,SemanticModel model)
        {
            var syntax = syntaxNode as CompilationUnitSyntax;
            var newItem = new RDomRoot(syntaxNode, parent,model);

            newItem.Name = "Root";
            CreateFromHelpers.LoadStemMembers(newItem, syntax.Members, syntax.Usings,model);

            return newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IRoot item)
        {
            var node = SyntaxFactory.CompilationUnit();
            var usingsSyntax = item.UsingDirectives
                        .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                        .ToList();
            var membersSyntax = item.StemMembers
                        .Where(x=>!(x is IUsingDirective))
                        .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithUsings(SyntaxFactory.List(usingsSyntax));
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            return item.PrepareForBuildSyntaxOutput(node);
        }
    }


}
