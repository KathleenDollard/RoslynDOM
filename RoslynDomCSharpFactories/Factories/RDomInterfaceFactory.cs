using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace RoslynDom.CSharp
{
    internal static class RDomInterfaceFactoryHelper
    {
        [ExcludeFromCodeCoverage]
        private static string nameof<T>(T value) { return ""; }

        private static WhitespaceKindLookup _whitespaceLookup;

        private static WhitespaceKindLookup whitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.Add(LanguageElement.InterfaceKeyword, SyntaxKind.InterfaceKeyword);
                    _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
                    _whitespaceLookup.Add(LanguageElement.InterfaceStartDelimiter, SyntaxKind.OpenBraceToken);
                    _whitespaceLookup.Add(LanguageElement.InterfaceEndDelimiter, SyntaxKind.CloseBraceToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.OopModifiers);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        public static RDomInterface CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model, ICSharpCreateFromWorker createFromWorker, RDomCorporation corporation)
        {
            var syntax = syntaxNode as InterfaceDeclarationSyntax;
            var newItem = new RDomInterface(syntaxNode, parent, model);
            createFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            createFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, whitespaceLookup);

            newItem.Name = newItem.TypedSymbol.Name;

            var members = ListUtilities.MakeList(syntax, x => x.Members, x => corporation.CreateFrom<ITypeMemberCommentWhite>(x, newItem, model));
            // this is a hack becuase the membersare appearing with a scope
            foreach (var member in members.OfType<ITypeMember>())
            { member.AccessModifier = AccessModifier.None; }
            newItem.MembersAll.AddOrMoveRange(members);

            return newItem;
        }

            public static IEnumerable<SyntaxNode> BuildSyntax(IDom item, ICSharpBuildSyntaxWorker buildSyntaxWorker, RDomCorporation corporation)
        {
            var itemAsInterface = item as IInterface;
            var modifiers = itemAsInterface.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(itemAsInterface.Name);
            var node = SyntaxFactory.InterfaceDeclaration(identifier)
                .WithModifiers(modifiers);
            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsInterface.Whitespace2Set, whitespaceLookup);

            var attributes = buildSyntaxWorker.BuildAttributeSyntax(itemAsInterface.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }
            Guardian.Assert.IsNotNull(itemAsInterface, nameof(itemAsInterface));
            var membersSyntax = itemAsInterface.Members
                        .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            //node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));
            // TODO: Class type members and type constraints
            return node.PrepareForBuildSyntaxOutput(item);
        }
    }

    public class RDomInterfaceTypeMemberFactory
       : RDomTypeMemberFactory<RDomInterface, InterfaceDeclarationSyntax>
    {
        public RDomInterfaceTypeMemberFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomInterfaceFactoryHelper.CreateFrom(syntaxNode, parent, model, CreateFromWorker, Corporation);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item, BuildSyntaxWorker, Corporation);
        }
    }


    public class RDomInterfaceStemMemberFactory
           : RDomStemMemberFactory<RDomInterface, InterfaceDeclarationSyntax>
    {
        public RDomInterfaceStemMemberFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomInterfaceFactoryHelper.CreateFrom(syntaxNode, parent, model, CreateFromWorker, Corporation);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item, BuildSyntaxWorker, Corporation);
        }
    }



}
