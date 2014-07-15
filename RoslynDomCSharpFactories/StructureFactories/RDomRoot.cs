using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomRootFactory
          : RDomRootContainerFactory<IRoot, CompilationUnitSyntax>
    {
        public override void InitializeItem(IRoot newItem, CompilationUnitSyntax syntax)
        {
            newItem.Name = "Root";
            var members = ListUtilities.MakeList(syntax, x => x.Members, x => RDomFactoryHelper.StemMemberFactoryHelper.MakeItem(x));
            var usings = ListUtilities.MakeList(syntax, x => x.Usings, x => RDomFactoryHelper.StemMemberFactoryHelper.MakeItem(x));
            foreach (var member in members)
            { newItem.AddOrMoveStemMember(member); }
            foreach (var member in usings)
            { newItem.AddOrMoveStemMember(member); }
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IRoot item)
        {
            var node = SyntaxFactory.CompilationUnit();
            var usingsSyntax = item.Usings
                        .SelectMany(x => RDomFactory.BuildSyntaxGroup(x))
                        .ToList();
            var membersSyntax = item.StemMembers
                        .Where(x=>!(x is IUsing))
                        .SelectMany(x => RDomFactory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithUsings(SyntaxFactory.List(usingsSyntax));
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }

}
