
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class TriviaManager
    {
        internal void StoreWhitespace(IDom newItem, SyntaxNode syntaxNode, LanguagePart languagePart, WhitespaceKindLookup whitespaceLookup)
        {
            if (syntaxNode == null) return;

            // For now, all expressions are held as strings, so we just care about first/last
            var nodeAsExpressionSyntax = syntaxNode as ExpressionSyntax;
            if (nodeAsExpressionSyntax is ExpressionSyntax)
            {
                StoreWhitespaceForExpression(newItem, nodeAsExpressionSyntax, languagePart);
                return;
            }

            var lookForIdentifier = whitespaceLookup.Lookup(LanguageElement.Identifier) != SyntaxKind.None;
            lookForIdentifier = StoreWhitespaceForChildren(newItem, syntaxNode,
                                    languagePart, whitespaceLookup, lookForIdentifier);
            if (lookForIdentifier)
            { StoreWhitespaceForIdentifierNode(newItem, syntaxNode, languagePart); }
        }

        private void StoreWhitespaceForExpression(IDom newItem, ExpressionSyntax syntaxNode,
               LanguagePart languagePart)
        {
            StoreWhitespaceForFirstAndLastToken(newItem, syntaxNode,
                    languagePart, LanguageElement.Expression);
        }

        private void StoreWhitespaceForIdentifierNode(IDom newItem, SyntaxNode syntaxNode,
                    LanguagePart languagePart)
        {
            // assume if it was a token we already found it
            var idNode = syntaxNode.ChildNodes().OfType<NameSyntax>().FirstOrDefault();
            if (idNode != null)
            {
                StoreWhitespaceForFirstAndLastToken(newItem, idNode,
                        languagePart, LanguageElement.Identifier);
            }
        }

        private bool StoreWhitespaceForChildren(IDom newItem, SyntaxNode syntaxNode,
                    LanguagePart languagePart, WhitespaceKindLookup whitespaceLookup, bool lookForIdentifier)
        {
            foreach (var token in syntaxNode.ChildTokens())
            {
                var kind = token.CSharpKind();
                var languageElement = whitespaceLookup.Lookup(kind);
                if (languageElement == LanguageElement.Identifier)
                { lookForIdentifier = false; }
                if (languageElement != LanguageElement.NotApplicable)
                { StoreWhitespaceForToken(newItem, token, languagePart, languageElement); }
            }

            return lookForIdentifier;
        }

        internal void StoreWhitespaceForToken(IDom newItem, SyntaxToken token,
                    LanguagePart languagePart, LanguageElement languageElement)
        {
            var newWS = new Whitespace2(LanguagePart.Current, languageElement);
            var region = token.LeadingTrivia.Where(x => x.CSharpKind() == SyntaxKind.RegionDirectiveTrivia).FirstOrDefault();
            var structure = region.GetStructure();
            newWS.LeadingWhitespace = token.LeadingTrivia
                                        .Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia)
                                        .Select(x => x.ToString())
                                        .JoinString();
            newWS.TrailingWhitespace = token.TrailingTrivia
                                        .Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia
                                                || x.CSharpKind() == SyntaxKind.EndOfLineTrivia)
                                        .Select(x => x.ToString())
                                        .JoinString();
            // TODO: Add EOL comments here
            newItem.Whitespace2Set[languageElement] = newWS;
        }

        public void StoreWhitespaceForFirstAndLastToken(IDom newItem, SyntaxNode node,
                LanguagePart languagePart,
                LanguageElement languageElement)
        {
            StoreWhitespaceForToken(newItem, node.GetFirstToken(), languagePart, languageElement);
            StoreWhitespaceForToken(newItem, node.GetLastToken(), languagePart, languageElement);
        }
    }
}
