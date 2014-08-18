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
    public class RDomEnumMemberMiscFactory
       : RDomMiscFactory<RDomEnumMember, EnumMemberDeclarationSyntax>
    {
        // until move to C# 6 - I want to support name of as soon as possible
        [ExcludeFromCodeCoverage]
        private static string nameof<T>(T value) { return ""; }

        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomEnumMemberMiscFactory(RDomCorporation corporation)
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
                    _whitespaceLookup.Add(LanguageElement.EnumValueAssignOperator, SyntaxKind.EqualsToken);
                    _whitespaceLookup.Add(LanguageElement.EnumValueSeparator, SyntaxKind.CommaToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as EnumMemberDeclarationSyntax;
            var newItem = new RDomEnumMember(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            MemberWhitespace(newItem, syntax);

            newItem.Name = syntax.Identifier.ToString();
            if (syntax.EqualsValue != null)
            {
                newItem.Expression = Corporation
                    .CreateFrom<IExpression>(syntax.EqualsValue.Value, newItem, model)
                    .FirstOrDefault();
            }
            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IEnumMember;
            Guardian.Assert.IsNotNull(itemAsT, nameof(itemAsT));

            var identifier = SyntaxFactory.Identifier(itemAsT.Name);
            var node = SyntaxFactory.EnumMemberDeclaration(identifier);

            var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

            if (itemAsT.Expression != null)
            {
                var exprSyntax = SyntaxFactory.ParseExpression(itemAsT.Expression.Expression);
                var equalsValueSyntax = SyntaxFactory.EqualsValueClause(exprSyntax);
                equalsValueSyntax = BuildSyntaxHelpers.AttachWhitespace(equalsValueSyntax, item.Whitespace2Set, WhitespaceLookup);
                node = node.WithEqualsValue(equalsValueSyntax);
            }

            // node = BuildSyntaxHelpers.AttachWhitespace(node, item.Whitespace2Set, WhitespaceLookup);
            node = BuildSyntaxHelpers.AttachWhitespaceToFirst(node, item.Whitespace2Set[LanguageElement.EnumValueFirstToken]);
            node = BuildSyntaxHelpers.AttachWhitespaceToLast(node, item.Whitespace2Set[LanguageElement.EnumValueLastToken]);
            return node.PrepareForBuildSyntaxOutput(item);
        }

        private void MemberWhitespace(RDomEnumMember newItem, EnumMemberDeclarationSyntax syntax)
        {
            CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.GetFirstToken(), LanguagePart.Current, LanguageElement.EnumValueFirstToken);
            CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.GetLastToken(), LanguagePart.Current, LanguageElement.EnumValueLastToken);
            if (syntax.EqualsValue != null)
            {
                CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.EqualsValue.Value.GetLastToken(), LanguagePart.Current, LanguageElement.Identifier);
                CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.EqualsValue.EqualsToken, LanguagePart.Current, LanguageElement.EnumValueAssignOperator);
            }

            var prevNodeOrToken = syntax.Parent
                                    .ChildNodesAndTokens()
                                    .PreviousSiblings(syntax)
                                    .LastOrDefault();
            var sepKind = WhitespaceLookup.Lookup(LanguageElement.EnumValueSeparator);
            if (prevNodeOrToken.CSharpKind() == sepKind)
            {
                var commaToken = prevNodeOrToken.AsToken();
                var whitespace2 = newItem.Whitespace2Set[LanguageElement.EnumValueFirstToken];
                if (string.IsNullOrEmpty(whitespace2.LeadingWhitespace))
                { whitespace2.LeadingWhitespace = commaToken.TrailingTrivia.ToString(); }
            }
        }
    }



}
