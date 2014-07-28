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
        public RDomPropertyAccessorMiscFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as AccessorDeclarationSyntax;
            var parentProperty = parent as IProperty;
            var accessorType = (syntaxNode.CSharpKind() == SyntaxKind.GetAccessorDeclaration)
                                ? AccessorType.Get : AccessorType.Set;
            var newItem = new RDomPropertyAccessor(syntaxNode, accessorType, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);

            var newItemName = accessorType.ToString().ToLower() + "_" + parentProperty.Name;
            newItem.Name = newItemName;

            return  newItem ;

        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsAccessor = item as IAccessor;
            var parentProperty = item.Parent as IProperty;
            if (itemAsAccessor == null || parentProperty == null) { throw new InvalidOperationException(); }
            var statements = BuildSyntaxWorker.GetStatementBlock(itemAsAccessor.Statements);
            var kind = (itemAsAccessor.AccessorType == AccessorType.Get)
                        ? SyntaxKind.GetAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;
            var node = SyntaxFactory.AccessorDeclaration(kind, statements);

            var attributeList = BuildSyntaxWorker.BuildAttributeSyntax(itemAsAccessor.Attributes);
            if (attributeList.Any()) { node = node.WithAttributeLists(attributeList); }

            return item.PrepareForBuildSyntaxOutput(node);
        }

    }

}
