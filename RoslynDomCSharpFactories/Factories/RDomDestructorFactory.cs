using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomDestructorTypeMemberFactory
          : RDomTypeMemberFactory<RDomDestructor, DestructorDeclarationSyntax>
    {
        public RDomDestructorTypeMemberFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as DestructorDeclarationSyntax;
            var newItem = new RDomDestructor(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);

            newItem.Name = newItem.TypedSymbol.Name;

            newItem.AccessModifier = RoslynUtilities.GetAccessibilityFromSymbol(newItem.Symbol);

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IDestructor;
            var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);

            var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
            var node = SyntaxFactory.DestructorDeclaration(nameSyntax)
                            .WithModifiers(modifiers);

            var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

            node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

            //node = node.WithBody(RoslynCSharpUtilities.MakeStatementBlock(itemAsT.Statements));
            node = node.WithBody((BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT));

            return node.PrepareForBuildSyntaxOutput(item);
        }

    }


}
