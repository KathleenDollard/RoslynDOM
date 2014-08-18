using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;
using System;

namespace RoslynDom.CSharp
{
    public class RDomParameterMiscFactory
            : RDomMiscFactory<RDomParameter, ParameterSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomParameterMiscFactory(RDomCorporation corporation)
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
                    _whitespaceLookup.Add(LanguageElement.ParameterDefaultAssignOperator, SyntaxKind.ColonToken);
                    _whitespaceLookup.Add(LanguageElement.ParameterSeparator, SyntaxKind.CommaToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }
        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ParameterSyntax;
            var newItem = new RDomParameter(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            MemberWhitespace(newItem, syntax);

            newItem.Name = newItem.TypedSymbol.Name;

            //newItem.Type = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.Type);
            var type = Corporation
                             .CreateFrom<IMisc>(syntax.Type, newItem, model)
                             .FirstOrDefault()
                             as IReferencedType;
            newItem.Type = type;

            newItem.IsOut = newItem.TypedSymbol.RefKind == RefKind.Out;
            newItem.IsRef = newItem.TypedSymbol.RefKind == RefKind.Ref;
            newItem.IsParamArray = newItem.TypedSymbol.IsParams;
            newItem.IsOptional = newItem.TypedSymbol.IsOptional;
            newItem.Ordinal = newItem.TypedSymbol.Ordinal;

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IParameter;
            Guardian.Assert.IsNotNull(itemAsT, nameof(itemAsT));

            var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);
            var syntaxType = (TypeSyntax)(RDomCSharp.Factory.BuildSyntax(itemAsT.Type));

            var node = SyntaxFactory.Parameter(nameSyntax)
                        .WithType(syntaxType);

            var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsT.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

            var modifiers = SyntaxFactory.TokenList();
            if (itemAsT.IsOut) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.OutKeyword)); }
            if (itemAsT.IsRef) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.RefKeyword)); }
            if (itemAsT.IsParamArray) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ParamsKeyword)); }
            if (itemAsT.IsRef) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.RefKeyword)); }
            if (modifiers.Any()) { node = node.WithModifiers(modifiers); }

           // node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            node = BuildSyntaxHelpers.AttachWhitespaceToFirst(node, item.Whitespace2Set[LanguageElement.ParameterFirstToken ]);
            node = BuildSyntaxHelpers.AttachWhitespaceToLast(node, item.Whitespace2Set[LanguageElement.ParameterLastToken]);
            return node.PrepareForBuildSyntaxOutput(item);

        }
        private void MemberWhitespace(RDomParameter newItem, ParameterSyntax  syntax)
        {
            CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.GetFirstToken(), LanguagePart.Current, LanguageElement.ParameterFirstToken);
            CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.GetLastToken(), LanguagePart.Current, LanguageElement.ParameterLastToken);
            if (syntax.Default != null)
            {
                CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.Default.Value.GetLastToken(), LanguagePart.Current, LanguageElement.Identifier);
                CreateFromWorker.StoreWhitespaceForToken(newItem, syntax.Default.EqualsToken, LanguagePart.Current, LanguageElement.ParameterDefaultAssignOperator);
            }

            var prevNodeOrToken = syntax.Parent
                                    .ChildNodesAndTokens()
                                    .PreviousSiblings(syntax)
                                    .LastOrDefault();
            var sepKind = WhitespaceLookup.Lookup(LanguageElement.ParameterSeparator);
            if (prevNodeOrToken.CSharpKind() == sepKind)
            {
                var commaToken = prevNodeOrToken.AsToken();
                var whitespace2 = newItem.Whitespace2Set[LanguageElement.ParameterFirstToken];
                if (string.IsNullOrEmpty(whitespace2.LeadingWhitespace))
                { whitespace2.LeadingWhitespace = commaToken.TrailingTrivia.ToString(); }
            }
        }
    }

}
