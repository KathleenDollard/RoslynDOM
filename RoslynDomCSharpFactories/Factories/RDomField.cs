using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomFieldTypeMemberFactory
          : RDomTypeMemberFactory<RDomField, FieldDeclarationSyntax>
    {
        public override void InitializeItem(RDomField newItem, FieldDeclarationSyntax syntax)
        {
            newItem.Name = newItem.TypedSymbol.Name;
            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
            newItem.ReturnType = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.Type);
            newItem.IsStatic = newItem.Symbol.IsStatic;
        }
        public override IEnumerable<ITypeMember> CreateFrom(SyntaxNode syntaxNode)
        {
            var list = new List<ITypeMember>();
            // We can't do this in the constructor, because many may be created and we want to flatten
            var rawField = syntaxNode as FieldDeclarationSyntax;
            var declarators = rawField.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = new RDomField(rawField, decl);
                list.Add(newItem);
                InitializeItem(newItem, rawField);
            }
            return list;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            var nameSyntax = SyntaxFactory.Identifier(item.Name);
            var itemAsField  = item as IField;
            var returnType = (TypeSyntax)RDomFactory.BuildSyntaxGroup(itemAsField.ReturnType).First();
            var modifiers = BuildSyntaxExtensions.BuildModfierSyntax(item);
            var declaratorNode = SyntaxFactory.VariableDeclarator(nameSyntax);
            var variableNode = SyntaxFactory.VariableDeclaration(returnType)
               .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(nameSyntax)));
            var node = SyntaxFactory.FieldDeclaration(variableNode)
               .WithModifiers(modifiers);
            var attributes = BuildSyntaxExtensions.BuildAttributeListSyntax(item.Attributes );
            if (attributes.Any()) { node = node.WithAttributeLists(attributes );}
            return new SyntaxNode[] { node.NormalizeWhitespace() };
        }

     }
     
}
