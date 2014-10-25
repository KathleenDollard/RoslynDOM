using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomDestructorTypeMemberFactory
          : RDomBaseSyntaxNodeFactory<RDomDestructor, DestructorDeclarationSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomDestructorTypeMemberFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
            if (_whitespaceLookup == null)
            {
                _whitespaceLookup = new WhitespaceKindLookup();
                _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
                _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as DestructorDeclarationSyntax;
            var newItem = new RDomDestructor(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model, OutputContext);
            CreateFromWorker.InitializeStatements(newItem, syntax.Body, newItem, model);
            CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);

            newItem.Name = newItem.TypedSymbol.Name;

            return newItem;
        }

             public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IDestructor;
            var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);

            var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsT);
            var node = SyntaxFactory.DestructorDeclaration(nameSyntax)
                            .WithModifiers(modifiers);
            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

            var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

            //node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

            node = node.WithBody((BlockSyntax)RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup));

            return node.PrepareForBuildSyntaxOutput(item, OutputContext );
        }

    }


}
