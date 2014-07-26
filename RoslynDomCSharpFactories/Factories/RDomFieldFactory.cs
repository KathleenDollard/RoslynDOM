using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomFieldTypeMemberFactory
          : RDomTypeMemberFactory<RDomField, VariableDeclaratorSyntax>
    {
        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            // This will conflict with Declaration statement if we don't scope factories. In that case, check parent
            return syntaxNode is FieldDeclarationSyntax;
        }

        protected  override IEnumerable<ITypeMemberCommentWhite> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var list = new List<ITypeMember>();

            var fieldPublicAnnotations = RDomFactoryHelper.GetPublicAnnotations(syntaxNode, parent, model);
            var rawField = syntaxNode as FieldDeclarationSyntax;
            var declarators = rawField.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = new RDomField(decl, parent, model);
                list.Add(newItem);
                newItem.Name = newItem.TypedSymbol.Name;

                var attributes = RDomFactoryHelper.CreateAttributeFrom(syntaxNode, newItem, model);
                newItem.Attributes.AddOrMoveAttributeRange(attributes);

                newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
                newItem.ReturnType = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.Type);
                newItem.IsStatic = newItem.Symbol.IsStatic;
                newItem.PublicAnnotations.Add(fieldPublicAnnotations);

            }
            return list;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMemberCommentWhite item)
        {
            var itemAsField = item as IField;
            var nameSyntax = SyntaxFactory.Identifier(itemAsField.Name);
            var returnType = (TypeSyntax)RDomCSharp.Factory.BuildSyntaxGroup(itemAsField.ReturnType).First();
            var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsField);
            var declaratorNode = SyntaxFactory.VariableDeclarator(nameSyntax);
            var variableNode = SyntaxFactory.VariableDeclaration(returnType)
               .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(nameSyntax)));
            var node = SyntaxFactory.FieldDeclaration(variableNode)
               .WithModifiers(modifiers);
            var attributes = RDomFactoryHelper.BuildAttributeSyntax(itemAsField.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes.WrapInAttributeList()); }

            node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

            return item.PrepareForBuildSyntaxOutput(node);
        }

    }

}
