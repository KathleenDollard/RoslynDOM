// Variation of the replacer in the C# Roslyn
// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;



namespace RoslynDom.CSharp
{
    public static class SyntaxReplacer
    {
        // TODO: Do variations later 
        public static TNode ReplaceToken<TNode>(this TNode node, SyntaxToken oldToken, SyntaxToken newToken)
             where TNode : SyntaxNode
        {
            return Replace(node, tokens: new[] { oldToken }, computeReplacementToken: (o, r) => newToken);
        }

        private static TNode Replace<TNode>(
            TNode node,
            IEnumerable<SyntaxNode> childNodes = null,
            Func<SyntaxNode, SyntaxNode, SyntaxNode> computeReplacementNode = null,
            IEnumerable<SyntaxToken> tokens = null,
            Func<SyntaxToken, SyntaxToken, SyntaxToken> computeReplacementToken = null,
            IEnumerable<SyntaxTrivia> trivia = null,
            Func<SyntaxTrivia, SyntaxTrivia, SyntaxTrivia> computeReplacementTrivia = null)
            where TNode : SyntaxNode
        {
            var replacer = new Replacer<TNode>(
                childNodes, computeReplacementNode,
                tokens, computeReplacementToken,
                trivia, computeReplacementTrivia);

            if (replacer.HasWork)
            {
                return replacer.Visit(node) as TNode;
            }
            else
            {
                return node;
            }
        }

        private class Replacer<TNode> : CSharpSyntaxRewriter
                 where TNode : SyntaxNode
        {
            private readonly Func<TNode, TNode, SyntaxNode> computeReplacementNode;
            private readonly Func<SyntaxToken, SyntaxToken, SyntaxToken> computeReplacementToken;
            private readonly Func<SyntaxTrivia, SyntaxTrivia, SyntaxTrivia> computeReplacementTrivia;

            private readonly HashSet<SyntaxNode> nodeSet;
            private readonly HashSet<SyntaxToken> tokenSet;
            private readonly HashSet<SyntaxTrivia> triviaSet;

            private readonly bool visitIntoStructuredTrivia;
            private readonly bool shouldVisitTrivia;

            private static readonly HashSet<SyntaxNode> NoNodes = new HashSet<SyntaxNode>();
            private static readonly HashSet<SyntaxToken> NoTokens = new HashSet<SyntaxToken>();
            private static readonly HashSet<SyntaxTrivia> NoTrivia = new HashSet<SyntaxTrivia>();

            public Replacer(
                IEnumerable<SyntaxNode> nodes,
                Func<SyntaxNode, SyntaxNode, SyntaxNode> computeReplacementNode,
                IEnumerable<SyntaxToken> tokens,
                Func<SyntaxToken, SyntaxToken, SyntaxToken> computeReplacementToken,
                IEnumerable<SyntaxTrivia> trivia,
                Func<SyntaxTrivia, SyntaxTrivia, SyntaxTrivia> computeReplacementTrivia)
            {
                this.computeReplacementNode = computeReplacementNode;
                this.computeReplacementToken = computeReplacementToken;
                this.computeReplacementTrivia = computeReplacementTrivia;

                this.nodeSet = nodes != null ? new HashSet<SyntaxNode>(nodes) : NoNodes;
                this.tokenSet = tokens != null ? new HashSet<SyntaxToken>(tokens) : NoTokens;
                this.triviaSet = trivia != null ? new HashSet<SyntaxTrivia>(trivia) : NoTrivia;

                this.shouldVisitTrivia = this.triviaSet.Count > 0 || this.visitIntoStructuredTrivia;
            }

            public bool HasWork
            {
                get
                {
                    return this.nodeSet.Count + this.tokenSet.Count + this.triviaSet.Count > 0;
                }
            }

            public override SyntaxNode Visit(SyntaxNode node)
            {
                SyntaxNode rewritten = node;

                if (node != null)
                {
                    rewritten = base.Visit(node);

                    if (this.nodeSet.Contains(node) && this.computeReplacementNode != null)
                    {
                        rewritten = this.computeReplacementNode((TNode)node, (TNode)rewritten);
                    }
                }

                return rewritten;
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                var rewritten = token;


                rewritten = base.VisitToken(token);

                if (this.tokenSet.Contains(token) && this.computeReplacementToken != null)
                {
                    rewritten = this.computeReplacementToken(token, rewritten);
                }

                return rewritten;
            }

            public override SyntaxTrivia VisitListElement(SyntaxTrivia trivia)
            {
                var rewritten = trivia;

                if (this.VisitIntoStructuredTrivia && trivia.HasStructure)
                {
                    rewritten = this.VisitTrivia(trivia);
                }

                if (this.triviaSet.Contains(trivia) && this.computeReplacementTrivia != null)
                {
                    rewritten = this.computeReplacementTrivia(trivia, rewritten);
                }

                return rewritten;
            }
        }
    }
}
