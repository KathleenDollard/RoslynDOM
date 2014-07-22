using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomPropertyAccessorMiscFactory
          : RDomMiscFactory<RDomPropertyAccessor, AccessorDeclarationSyntax>
    {
        public override IEnumerable<IMisc> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as AccessorDeclarationSyntax;
            var parentProperty = parent as IProperty;
            var accessorType = (syntaxNode.CSharpKind() == SyntaxKind.GetAccessorDeclaration)
                                ? AccessorType.Get : AccessorType.Set;
            var newItem = new RDomPropertyAccessor(syntaxNode, accessorType, parent, model);
            var newItemName = accessorType.ToString().ToLower() + "_" + parentProperty.Name;
            newItem.Name = newItemName;

            var attributes = RDomFactoryHelper.GetAttributesFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
            if (syntax.Body != null)
            {
                var statements = ListUtilities.MakeList(syntax, x => x.Body.Statements, x => RDomFactoryHelper.GetHelper<IStatement>().MakeItem(x, newItem, model));
                newItem.Statements.AddOrMoveRange(statements);
            }

            return new IMisc[] { newItem };

        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IMisc item)
        {
            var itemAsAccessor = item as IAccessor;
            var parentProperty = item.Parent as IProperty;
            if (itemAsAccessor == null || parentProperty == null) { throw new InvalidOperationException(); }
            var statements = RoslynCSharpUtilities.MakeStatementBlock(itemAsAccessor.Statements);
            var kind = (itemAsAccessor.AccessorType == AccessorType.Get)
                        ? SyntaxKind.GetAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;
            var node = SyntaxFactory.AccessorDeclaration(kind, statements);
            var attributes = RDomFactoryHelper.BuildAttributeSyntax(itemAsAccessor.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes.WrapInAttributeList()); }

            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }

    }

}
