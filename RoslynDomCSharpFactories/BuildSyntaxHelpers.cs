using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public static class BuildSyntaxHelpers
    {

        [ExcludeFromCodeCoverage]
        private static string nameof<T>(T value) { return ""; }

        private const string annotationMarker = "RDomBuildSyntaxMarker";

        public static IEnumerable<SyntaxNode> PrepareForBuildSyntaxOutput(this IEnumerable<SyntaxNode> nodes, IDom item)
        {
            var ret = new List<SyntaxNode>();
            foreach (var node in nodes)
            {
                ret.Add(PrepareForBuildItemSyntaxOutput(node, item));
            }
            return ret;
        }

        public static IEnumerable<SyntaxNode> PrepareForBuildSyntaxOutput(this SyntaxNode node, IDom item)
        {
            node = PrepareForBuildItemSyntaxOutput(node, item);
            return new SyntaxNode[] { node };
        }

        public static TNode PrepareForBuildItemSyntaxOutput<TNode>(this TNode node, IDom item)
            where TNode : SyntaxNode
        {
            var moreLeadingTrivia = BuildSyntaxHelpers.LeadingTrivia(item);
            var leadingTriviaList = moreLeadingTrivia.Concat(node.GetLeadingTrivia());
            node = node.WithLeadingTrivia(SyntaxFactory.TriviaList(leadingTriviaList));
            //if (!(node is AccessorDeclarationSyntax || node is PropertyDeclarationSyntax ))
            //{ node = BuildTokenWhitespace(node, item); }

            return node;
        }

        public static SyntaxNode Format(SyntaxNode node, IDom item)
        {
            var span = node.FullSpan;
            node = Formatter.Format(node, span, new CustomWorkspace());
            return node;
        }

        public static SyntaxList<AttributeListSyntax> WrapInAttributeList(IEnumerable<SyntaxNode> attributes)
        {
            var node = SyntaxFactory.List<AttributeListSyntax>(attributes.OfType<AttributeListSyntax>());
            return node;
        }

        public static SyntaxTokenList BuildModfierSyntax(this IHasAccessModifier hasAccessModifier)
        {
            var list = SyntaxFactory.TokenList();
            if (hasAccessModifier != null && hasAccessModifier.DeclaredAccessModifier != AccessModifier.None)
            { list = list.AddRange(SyntaxTokensForAccessModifier(hasAccessModifier.DeclaredAccessModifier)); }
            // TODO: Static and other modifiers
            return list;
        }


        ////public static TSyntax BuildTokenWhitespace<TSyntax>(TSyntax node, IDom item, bool useFirstLast)
        ////    where TSyntax : SyntaxNode
        ////{
        ////    // This is ugly because of side cases, so may need to be optimized for main line paths where all tokens are unique in context
        ////    // The ugliness is exhibited at least with the two semi-colons of a for-each.
        ////    var rDomItem = item as IRoslynDom;
        ////    if (rDomItem == null) return node;
        ////    if (rDomItem.TokenWhitespaceSet == null) return node; // correct at least for compilation root
        ////    var list = rDomItem.TokenWhitespaceSet.TokenWhitespaceList;
        ////    // in a few cases, there are multiple tokens of a type (for statement semicolons)
        ////    var tokens = node.ChildTokens();
        ////    var distinctTokenKinds = tokens.Select(x => x.CSharpKind()).Distinct();
        ////    var firstToken = node.DescendantTokens().First();
        ////    var lastToken = node.DescendantTokens().Last();
        ////    foreach (var kind in distinctTokenKinds)
        ////    {
        ////        var tokenWhitespaceMatches = list.Where(x => x.Token.CSharpKind() == kind).ToArray();
        ////        if (!tokenWhitespaceMatches.Any()) continue; // no whitespace information
        ////        var tokenMatches = tokens.Where(x => x.CSharpKind() == kind);
        ////        // match tokens from left, if whitespacematches run out, reuse first
        ////        for (int i = 0; i < tokenMatches.Count(); i++)
        ////        {
        ////            var tokenWhitespace = i < tokenWhitespaceMatches.Count()
        ////                                    ? tokenWhitespaceMatches[i]
        ////                                    : tokenWhitespaceMatches[0];
        ////            var token = tokenMatches.Skip(i).First();

        ////            SyntaxTriviaList newLeading;
        ////            SyntaxTriviaList newTrailing;
        ////            if (token == firstToken && token == lastToken) continue;

        ////            SyntaxToken newToken;
        ////            // For now, leading/trailing replace initial ws on token
        ////            if (useFirstLast && token == firstToken)
        ////            {
        ////                newTrailing = GetWhitespaceTriviaList(token.TrailingTrivia, tokenWhitespace.TrailingWhitespace);
        ////                newToken = token.WithTrailingTrivia(newTrailing);
        ////            }
        ////            else if (useFirstLast && token == lastToken)
        ////            {
        ////                newLeading = GetWhitespaceTriviaList(token.LeadingTrivia, tokenWhitespace.LeadingWhitespace);
        ////                newToken = token.WithLeadingTrivia(newLeading);
        ////            }
        ////            else
        ////            {
        ////                newLeading = GetWhitespaceTriviaList(token.LeadingTrivia, tokenWhitespace.LeadingWhitespace);
        ////                newTrailing = GetWhitespaceTriviaList(token.TrailingTrivia, tokenWhitespace.TrailingWhitespace);
        ////                newToken = token
        ////                     .WithLeadingTrivia(newLeading)
        ////                     .WithTrailingTrivia(newTrailing);
        ////            }
        ////            node = node.ReplaceToken(node.ChildTokens().Where(x => x.CSharpKind() == kind).Skip(i).First(), newToken);
        ////        }
        ////    }
        ////    if (useFirstLast)
        ////    {
        ////        SyntaxTriviaList trivia;
        ////        SyntaxToken descedantToken = node.DescendantTokens().First();
        ////        if (!string.IsNullOrEmpty(rDomItem.TokenWhitespaceSet.LeadingWhitespace))
        ////        {
        ////            descedantToken = node.DescendantTokens().First();
        ////            trivia = SyntaxFactory.ParseLeadingTrivia(rDomItem.TokenWhitespaceSet.LeadingWhitespace);
        ////            node = node.ReplaceToken(descedantToken, descedantToken.WithLeadingTrivia(descedantToken.LeadingTrivia.Concat(trivia)));
        ////        }

        ////        if (!string.IsNullOrEmpty(rDomItem.TokenWhitespaceSet.TrailingWhitespace))
        ////        {
        ////            descedantToken = node.DescendantTokens().Last();
        ////            trivia = SyntaxFactory.ParseTrailingTrivia(rDomItem.TokenWhitespaceSet.TrailingWhitespace);
        ////            node = node.ReplaceToken(descedantToken, descedantToken.WithTrailingTrivia(descedantToken.TrailingTrivia.Concat(trivia)));
        ////        }
        ////    }

        ////    //var rDomItem = item as IRoslynDom;
        ////    //if (rDomItem == null) return node;
        ////    //var list = rDomItem.TokenWhitespaceList.OfType<TokenWhitespaceCSharp>();
        ////    //foreach (var token in node.ChildTokens())
        ////    //{
        ////    //    var kind = (SyntaxKind)token.RawKind;
        ////    //    var tokenWhitespace = list.Where(x => x.Token.CSharpKind() == kind).SingleOrDefault();
        ////    //    if (tokenWhitespace != null)
        ////    //    {
        ////    //        var newToken = TokenWithWhitespace(tokenWhitespace, token, kind);
        ////    //        node = node.ReplaceToken(token, newToken);
        ////    //    }
        ////    //}


        ////    //var list = rDomItem.TokenWhitespaceList.OfType<TokenWhitespace<TSyntax>>();
        ////    //foreach (var tokenWhitespace in list)
        ////    //{
        ////    //    if (!string.IsNullOrEmpty(tokenWhitespace.LeadingWhitespace) || !string.IsNullOrEmpty(tokenWhitespace.TrailingWhitespace))
        ////    //    {
        ////    //        var oldToken = tokenWhitespace.Token;
        ////    //        var itemTokens = node.ChildTokens()
        ////    //                        .Where(x => x.RawKind == oldToken.RawKind);
        ////    //        if (itemTokens.Any())
        ////    //        {
        ////    //            var kind = (SyntaxKind)oldToken.RawKind;
        ////    //            var token = GetNewToken(tokenWhitespace, itemTokens.Single(), kind);
        ////    //            node = tokenWhitespace.WithDelegate(node, token);
        ////    //        }
        ////    //    }
        ////    //}
        ////    return node;
        ////}

        public static TSyntax BuildTokenWhitespace<TSyntax>(TSyntax node, IDom item)
                    where TSyntax : SyntaxNode
        {
            var rDomItem = item as IRoslynDom;
            if (rDomItem == null) return node;
            var tokenWhitespaceSet = rDomItem.TokenWhitespaceSet;
            if (tokenWhitespaceSet == null) return node; // correct at least for compilation root
            var list = tokenWhitespaceSet.TokenWhitespaceList;
            var includeLeadingTrailing = rDomItem.TokenWhitespaceSet.IncludeLeadingTrailing;
            if (list.Count() == 0 && !includeLeadingTrailing) return node; // often correct

            var replacer = new ReplaceWhitespace(list, node);
            node = (TSyntax)replacer.Visit(node)
                     .WithAdditionalAnnotations(RDomWhitespaceHandledAnnotation.Create());

            return node;
        }

        private class RDomWhitespaceHandledAnnotation
        {
            public const string Kind = "RDomWhitespaceHandled";
            public static SyntaxAnnotation Create()
            { return new SyntaxAnnotation(Kind); }
            //public static Guid GetMarker(SyntaxAnnotation annotation)
            //{ return Guid.Parse(annotation.Data); }
        }

        private class ReplaceWhitespace : CSharpSyntaxRewriter
        {
            private IEnumerable<TokenWhitespace> whitespaceList; // temporarily for reference
            private SyntaxNode parent;
            private IEnumerable<Tuple<SyntaxKind, List<TokenWhitespace>>> usedList;

            public ReplaceWhitespace(IEnumerable<TokenWhitespace> whitespaceList,
                SyntaxNode parent)
            {
                this.whitespaceList = whitespaceList;
                this.parent = parent;
                this.usedList = MakeUsedList(whitespaceList);
            }

            public override SyntaxNode Visit(SyntaxNode node)
            {
                SyntaxNode rewritten = node;
                if (rewritten != null)
                {
                    rewritten = base.Visit(rewritten);
                    if (node.Parent == parent && !node.HasAnnotations(RDomWhitespaceHandledAnnotation.Kind))
                    {
                        var tokenWs = GetTokenWhitespace(rewritten.CSharpKind());
                        if (tokenWs != null)
                        {
                            var leadingTrivia = GetLeadingWhitespaceTriviaList(rewritten.GetLeadingTrivia(), tokenWs.LeadingWhitespace);
                            var trailingTrivia = GetTrailingWhitespaceTriviaList(rewritten.GetTrailingTrivia(), tokenWs.TrailingWhitespace);
                            rewritten = rewritten
                                    .WithLeadingTrivia(leadingTrivia)
                                    .WithTrailingTrivia(trailingTrivia)
                                    .WithAdditionalAnnotations(RDomWhitespaceHandledAnnotation.Create()); ;
                        }
                    }
                }
                return rewritten;
            }

            private TokenWhitespace GetTokenWhitespace(SyntaxKind kind)
            {
                TokenWhitespace tokenWs = null;
                var matchingTuple = usedList.Where(x => x.Item1 == kind).FirstOrDefault();
                if (matchingTuple != null)
                {
                    tokenWs = matchingTuple.Item2.First(); // it it's there, it should have a non-empty list
                    if (matchingTuple.Item2.Count() > 1)
                    { matchingTuple.Item2.RemoveAt(0); }
                }
                return tokenWs;
            }

            public override SyntaxToken VisitToken(SyntaxToken token)
            {
                SyntaxToken rewritten = token;
                rewritten = base.VisitToken(rewritten);
                if (((token.Parent == parent) || ((token.Parent != null) && (token.Parent.Parent == parent)))
                    && !token.HasAnnotations(RDomWhitespaceHandledAnnotation.Kind))
                {
                    var tokenWs = GetTokenWhitespace(rewritten.CSharpKind());
                    if (tokenWs != null)
                    {
                        var leadingTrivia = GetLeadingWhitespaceTriviaList(rewritten.LeadingTrivia, tokenWs.LeadingWhitespace);
                        var trailingTrivia = GetTrailingWhitespaceTriviaList(rewritten.TrailingTrivia, tokenWs.TrailingWhitespace);
                        rewritten = rewritten
                                    .WithLeadingTrivia(leadingTrivia)
                                    .WithTrailingTrivia(trailingTrivia)
                                    .WithAdditionalAnnotations(RDomWhitespaceHandledAnnotation.Create());
                    }
                }
                return rewritten;
            }

            private static IEnumerable<Tuple<SyntaxKind, List<TokenWhitespace>>> MakeUsedList(IEnumerable<TokenWhitespace> list)
            {
                var usedList = new List<Tuple<SyntaxKind, List<TokenWhitespace>>>();
                var groups = list
                            .GroupBy(x => x.NodeOrToken.CSharpKind());
                foreach (var group in groups)
                {
                    usedList.Add(Tuple.Create(group.Key, new List<TokenWhitespace>(group)));
                }
                return usedList;
            }
        }

        //public static TSyntax BuildTokenWhitespace<TSyntax>(TSyntax node, IDom item)
        //            where TSyntax : SyntaxNode
        //{
        //    var rDomItem = item as IRoslynDom;
        //    if (rDomItem == null) return node;
        //    var tokenWhitespaceSet = rDomItem.TokenWhitespaceSet;
        //    if (tokenWhitespaceSet == null) return node; // correct at least for compilation root
        //    var list = tokenWhitespaceSet.TokenWhitespaceList;
        //    var includeLeadingTrailing = rDomItem.TokenWhitespaceSet.IncludeLeadingTrailing;
        //    if (list.Count() == 0 && !includeLeadingTrailing) return node; // often correct

        //    node = MarkChildTokens(node, list);
        //    var replacer = new ReplaceWhitespace(list);
        //    node = (TSyntax)replacer.Visit(node);

        //    return node;
        //}

        //private class RDomBuildSyntaxAnnotation
        //{
        //    public const string Kind = "RDomBuildSyntax";
        //    public static SyntaxAnnotation Create(Guid guid)
        //    { return new SyntaxAnnotation(Kind, guid.ToString()); }
        //    public static Guid GetMarker(SyntaxAnnotation annotation)
        //    { return Guid.Parse(annotation.Data); }
        //}

        //private class ReplaceWhitespace : CSharpSyntaxRewriter
        //{
        //    private IEnumerable<TokenWhitespace> whitespaceList;
        //    public ReplaceWhitespace(IEnumerable<TokenWhitespace> whitespaceList)
        //    { this.whitespaceList = whitespaceList; }

        //    public override SyntaxNode Visit(SyntaxNode node)
        //    {
        //        SyntaxNode rewritten = node;
        //        if (rewritten != null)
        //        {
        //            rewritten = base.Visit(rewritten);
        //            var tokenWs = GetTokenWhitespace(rewritten.GetAnnotations(RDomBuildSyntaxAnnotation.Kind));
        //            if (tokenWs != null)
        //            {
        //                var leadingTrivia = GetLeadingWhitespaceTriviaList(rewritten.GetLeadingTrivia(), tokenWs.LeadingWhitespace);
        //                var trailingTrivia = GetTrailingWhitespaceTriviaList(rewritten.GetTrailingTrivia(), tokenWs.TrailingWhitespace);
        //                rewritten = rewritten
        //                            .WithLeadingTrivia(leadingTrivia)
        //                            .WithTrailingTrivia(trailingTrivia);
        //            }
        //        }
        //        return rewritten;
        //    }

        //    private TokenWhitespace GetTokenWhitespace(IEnumerable<SyntaxAnnotation> annotations)
        //    {
        //        var annot = annotations.SingleOrDefault();
        //        if (annot == null) return null;
        //        var marker = RDomBuildSyntaxAnnotation.GetMarker(annot);
        //        var tokenWs = whitespaceList.Where(x => x.Marker == marker).FirstOrDefault ();
        //        return tokenWs;
        //    }

        //    public override SyntaxToken VisitToken(SyntaxToken token)
        //    {
        //        SyntaxToken rewritten = token;
        //        rewritten = base.VisitToken(rewritten);
        //        var tokenWs = GetTokenWhitespace(rewritten.GetAnnotations(RDomBuildSyntaxAnnotation.Kind));
        //        if (tokenWs != null)
        //        {
        //            var leadingTrivia = GetLeadingWhitespaceTriviaList(rewritten.LeadingTrivia, tokenWs.LeadingWhitespace);
        //            var trailingTrivia = GetTrailingWhitespaceTriviaList(rewritten.TrailingTrivia, tokenWs.TrailingWhitespace);
        //            rewritten = rewritten
        //                        .WithLeadingTrivia(leadingTrivia)
        //                        .WithTrailingTrivia(trailingTrivia);
        //        }
        //        return rewritten;
        //    }
        //}

        //private static TSyntax MarkChildTokens<TSyntax>(TSyntax node, List<TokenWhitespace> list) where TSyntax : SyntaxNode
        //{
        //    // NOTE!! Copy the whitespace list before calling this method. 
        //    var usedList = MakeUsedList(list);

        //    var childNodesOrTokens = node.ChildNodesAndTokens();
        //    // match tokens from left, if whitespacematches run out, reuse first
        //    for (int i = 0; i < childNodesOrTokens.Count(); i++)
        //    {
        //        var nodeOrToken = childNodesOrTokens.Skip(i).First();
        //        var kind = nodeOrToken.CSharpKind();
        //        var matches = list.Where(x => x.NodeOrToken.CSharpKind() == kind);
        //        var count = matches.Count();
        //        TokenWhitespace match;
        //        switch (count)
        //        {
        //            case 0: continue;
        //            case 1:
        //                {
        //                    match = matches.First();
        //                    break;
        //                }
        //            default:
        //                {
        //                    match = list.Where(x => x.Marker == Guid.Empty).FirstOrDefault();
        //                    if (match == null) match = matches.First();
        //                    break;
        //                }
        //        }

        //        Guid marker = match.Marker;
        //        node = node.ReplaceNodeOrToken(childNodesOrTokens.Skip(i).First(), nodeOrToken.WithAdditionalAnnotations(RDomBuildSyntaxAnnotation.Create(marker)));
        //    }

        //    return node;
        //}

        //public static TSyntax BuildTokenWhitespace2<TSyntax>(TSyntax node, IDom item)
        //    where TSyntax : SyntaxNode
        //{
        //    var rDomItem = item as IRoslynDom;
        //    if (rDomItem == null) return node;
        //    var tokenWhitespaceSet = rDomItem.TokenWhitespaceSet;
        //    if (tokenWhitespaceSet == null) return node; // correct at least for compilation root
        //    var list = tokenWhitespaceSet.TokenWhitespaceList;
        //    var includeLeadingTrailing = rDomItem.TokenWhitespaceSet.IncludeLeadingTrailing;
        //    if (list.Count() == 0 && !includeLeadingTrailing) return node; // often correct

        //    var usedList = MakeUsedList(list);

        //    var childNodesOrTokens = node.ChildNodesAndTokens();
        //    // match tokens from left, if whitespacematches run out, reuse first
        //    for (int i = 0; i < childNodesOrTokens.Count(); i++)
        //    {
        //        var nodeOrToken = childNodesOrTokens.Skip(i).First();
        //        var kind = childNodesOrTokens.Skip(i).First().CSharpKind();
        //        var usedItem = usedList.Where(x => x.Item1 == kind).FirstOrDefault();
        //        if (usedItem == null) continue; // probably need default ws later
        //        var tokenWhitespace = usedItem.Item2.Last(); // if it's there it should have an item
        //        if (usedItem.Item2.Count() > 1) usedItem.Item2.RemoveAt(usedItem.Item2.Count() - 1);

        //        var newLeading = GetLeadingWhitespaceTriviaList(nodeOrToken.GetLeadingTrivia(), tokenWhitespace.LeadingWhitespace);
        //        var newTrailing = GetTrailingWhitespaceTriviaList(nodeOrToken.GetTrailingTrivia(), tokenWhitespace.TrailingWhitespace);
        //        var newToken = nodeOrToken
        //                .AsToken()
        //                .WithLeadingTrivia(newLeading)
        //                .WithTrailingTrivia(newTrailing);
        //        node = node.ReplaceToken(node.ChildNodesAndTokens().Skip(i).First().AsToken(), newToken);
        //    }
        //    if (includeLeadingTrailing)
        //    {
        //        if (!string.IsNullOrEmpty(rDomItem.TokenWhitespaceSet.LeadingWhitespace))
        //        {
        //            var descedantToken = node.DescendantTokens().First();
        //            var trivia = SyntaxFactory.ParseLeadingTrivia(rDomItem.TokenWhitespaceSet.LeadingWhitespace);
        //            node = node.ReplaceNodeOrToken(node.DescendantTokens().First(), descedantToken.WithLeadingTrivia(descedantToken.LeadingTrivia.Concat(trivia)));
        //        }

        //        if (!string.IsNullOrEmpty(rDomItem.TokenWhitespaceSet.TrailingWhitespace))
        //        {
        //            var descedantToken = node.DescendantTokens().Last();
        //            var trivia = SyntaxFactory.ParseTrailingTrivia(rDomItem.TokenWhitespaceSet.TrailingWhitespace);
        //            node = node.ReplaceNodeOrToken(node.DescendantTokens().Last(), descedantToken.WithTrailingTrivia(descedantToken.TrailingTrivia.Concat(trivia)));
        //        }
        //    }

        //    return node;
        //}

        //private static List<Tuple<SyntaxKind, List<TokenWhitespace>>> MakeUsedList(List<TokenWhitespace> list)
        //{
        //    var usedList = new List<Tuple<SyntaxKind, List<TokenWhitespace>>>();
        //    var groups = list
        //                .GroupBy(x => x.NodeOrToken.CSharpKind());
        //    foreach (var group in groups)
        //    {
        //        usedList.Add(Tuple.Create(group.Key, new List<TokenWhitespace>(group)));
        //    }
        //    return usedList;
        //}


        ////private static void AnnotateTokenAligment(SyntaxNode node, IRoslynDom rDomItem)
        ////{
        ////    if (rDomItem.TokenWhitespaceSet == null) return; // correct at least for compilation root
        ////    var list = rDomItem.TokenWhitespaceSet.TokenWhitespaceList;
        ////    // in a few cases, there are multiple tokens of a type (for statement semicolons)
        ////    var tokens = node.ChildTokens();
        ////    var distinctTokenKinds = tokens.Select(x => x.CSharpKind()).Distinct();
        ////    foreach (var kind in distinctTokenKinds)
        ////    {
        ////        var tokenWhitespaceMatches = list.Where(x => x.Token.CSharpKind() == kind);
        ////        if (!tokenWhitespaceMatches.Any()) continue; // no whitespace information
        ////        var tokenMatches = tokens.Where(x => x.CSharpKind() == kind);
        ////        // match tokens from left, if whitespacematches run out, reuse first
        ////        for (int i = 0; i < tokenMatches.Count(); i++)
        ////        {
        ////            if (i >= tokenWhitespaceMatches.Count())
        ////            {

        ////            }
        ////            else
        ////            {

        ////            }
        ////            var tokenWhitespace = i < tokenWhitespaceMatches.Count()
        ////                                    ? tokenWhitespaceMatches[i]
        ////                                    : tokenWhitespaceMatches[0];
        ////            var token = tokenMatches.Skip(i).First();

        ////            SyntaxTriviaList newLeading;
        ////            SyntaxTriviaList newTrailing;
        ////            if (token == firstToken && token == lastToken) continue;

        ////            SyntaxToken newToken;
        ////            // For now, leading/trailing replace initial ws on token
        ////            if (useFirstLast && token == firstToken)
        ////            {
        ////                newTrailing = GetWhitespaceTriviaList(token.TrailingTrivia, tokenWhitespace.TrailingWhitespace);
        ////                newToken = token.WithTrailingTrivia(newTrailing);
        ////            }
        ////            else if (useFirstLast && token == lastToken)
        ////            {
        ////                newLeading = GetWhitespaceTriviaList(token.LeadingTrivia, tokenWhitespace.LeadingWhitespace);
        ////                newToken = token.WithLeadingTrivia(newLeading);
        ////            }
        ////            else
        ////            {
        ////                newLeading = GetWhitespaceTriviaList(token.LeadingTrivia, tokenWhitespace.LeadingWhitespace);
        ////                newTrailing = GetWhitespaceTriviaList(token.TrailingTrivia, tokenWhitespace.TrailingWhitespace);
        ////                newToken = token
        ////                     .WithLeadingTrivia(newLeading)
        ////                     .WithTrailingTrivia(newTrailing);
        ////            }
        ////            node = node.ReplaceToken(node.ChildTokens().Where(x => x.CSharpKind() == kind).Skip(i).First(), newToken);
        ////        }
        ////    }
        ////}

        private static SyntaxToken TokenWithWhitespace(TokenWhitespace tokenWhitespace,
            SyntaxToken itemToken, SyntaxKind kind)
        {
            var newLeading = GetLeadingWhitespaceTriviaList(itemToken.LeadingTrivia, tokenWhitespace.LeadingWhitespace);
            var newTrailing = GetTrailingWhitespaceTriviaList(itemToken.TrailingTrivia, tokenWhitespace.TrailingWhitespace);

            //var newLeading = itemToken.LeadingTrivia;
            //if (!string.IsNullOrEmpty(tokenWhitespace.LeadingWhitespace))
            //{ newLeading = newLeading.AddRange(SyntaxFactory.ParseLeadingTrivia(tokenWhitespace.LeadingWhitespace)); }

            //var newTrailing = itemToken.TrailingTrivia;
            //if (!string.IsNullOrEmpty(tokenWhitespace.TrailingWhitespace))
            //{ newTrailing = newTrailing.AddRange(SyntaxFactory.ParseTrailingTrivia(tokenWhitespace.TrailingWhitespace)); }

            var newToken = SyntaxFactory.Token(newLeading, kind, newTrailing);

            return newToken;
        }

        private static SyntaxTriviaList GetLeadingWhitespaceTriviaList(SyntaxTriviaList existingTriviaList, string tokenWhitespace)
        {
            var newTriviaList = existingTriviaList;
            if (!string.IsNullOrEmpty(tokenWhitespace))
            {
                var newWhitespaceList = SyntaxFactory.ParseLeadingTrivia(tokenWhitespace);
                var existingWhite = newTriviaList.Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia);
                foreach (var white in existingWhite)
                { newTriviaList = newTriviaList.Remove(white); }
                newTriviaList = newTriviaList.AddRange(newWhitespaceList);
            }
            return newTriviaList;
        }

        private static SyntaxTriviaList GetTrailingWhitespaceTriviaList(SyntaxTriviaList existingTriviaList, string tokenWhitespace)
        {
            var newTriviaList = existingTriviaList;
            if (!string.IsNullOrEmpty(tokenWhitespace))
            {
                var newWhitespaceList = SyntaxFactory.ParseTrailingTrivia(tokenWhitespace);
                var existingWhite = newTriviaList.Where(x => x.CSharpKind() == SyntaxKind.WhitespaceTrivia);
                foreach (var white in existingWhite)
                { newTriviaList = newTriviaList.Remove(white); }
                newTriviaList = newTriviaList.AddRange(newWhitespaceList);
            }
            return newTriviaList;
        }

        public static SyntaxTriviaList LeadingTrivia(IDom item)
        {
            var leadingTrivia = new List<SyntaxTrivia>();

            leadingTrivia.AddRange(BuildSyntaxHelpers.BuildStructuredDocumentationSyntax(item as IHasStructuredDocumentation));

            leadingTrivia.AddRange(BuildCommentWhite(item));

            return SyntaxFactory.TriviaList(leadingTrivia);
        }


        private static IEnumerable<SyntaxTrivia> BuildCommentWhite(IDom item)
        {
            var ret = new List<SyntaxTrivia>();
            // This can happen if someone copies an item to a new item, does not attach it to a tree, 
            // and asks for the syntax. It's actually expected to sometimes be unattached. 
            if (item.Parent == null) { return ret; }
            if (TryBuildCommentWhiteFor<IStemMemberCommentWhite, IStemContainer>(item, ret, x => x.StemMembersAll)) { return ret; }
            if (TryBuildCommentWhiteFor<ITypeMemberCommentWhite, ITypeMemberContainer>(item, ret, x => x.MembersAll)) { return ret; }
            if (TryBuildCommentWhiteFor<IStatementCommentWhite, IStatementContainer>(item, ret, x => x.StatementsAll)) { return ret; }
            return ret;
        }

        private static bool TryBuildCommentWhiteFor<TKind, TParent>
                    (IDom item, List<SyntaxTrivia> trivias, Func<TParent, IEnumerable<TKind>> getCandidates)
            where TParent : class
            where TKind : class
        {
            var itemAsTKind = item as TKind;
            if (itemAsTKind == null) { return false; }
            // if item is TKind, parent may not be TParent because types can be multiply rooted (stem or nested type)
            var parentAsTParent = item.Parent as TParent;
            if (parentAsTParent == null) return false;
            var candidates = getCandidates(parentAsTParent);
            var commentWhites = candidates
                                .PreviousSiblingsUntil(itemAsTKind, x => !(x is IComment || x is IVerticalWhitespace))
                                .OfType<ICommentWhite>();
            trivias.AddRange(MakeWhiteCommentTrivia(commentWhites));
            return true;
        }

        private static IEnumerable<SyntaxTrivia> MakeWhiteCommentTrivia(IEnumerable<ICommentWhite> commentWhites)
        {
            var ret = new List<SyntaxTrivia>();
            foreach (var item in commentWhites)
            {
                if (item is IVerticalWhitespace) { ret.Add(SyntaxFactory.EndOfLine("\r\n")); }
                else
                {
                    var itemAsComment = item as IComment;
                    Guardian.Assert.IsNotNull(itemAsComment, nameof(itemAsComment));
                    var comment = "";
                    if (itemAsComment.IsMultiline) { comment = "/* " + itemAsComment.Text + "*/"; }
                    else { comment = "// " + itemAsComment.Text; }
                    ret.Add(SyntaxFactory.Comment(comment));
                    ret.Add(SyntaxFactory.EndOfLine("\r\n"));
                }
            }
            return ret;
        }

        internal static SyntaxToken GetTokenFromKind(LiteralKind kind, object value)
        {
            switch (kind)
            {
                case LiteralKind.String:
                case LiteralKind.Unknown:
                    return SyntaxFactory.Literal(value.ToString());
                case LiteralKind.Numeric:
                    if (GeneralUtilities.IsInteger(value))
                    { return SyntaxFactory.Literal(Convert.ToInt32(value)); }
                    if (GeneralUtilities.IsFloatingPint(value))
                    { return SyntaxFactory.Literal(Convert.ToDouble(value)); }
                    if (value is uint)
                    { return SyntaxFactory.Literal(Convert.ToUInt32(value)); }
                    if (value is long)
                    { return SyntaxFactory.Literal(Convert.ToInt64(value)); }
                    if (value is ulong)
                    { return SyntaxFactory.Literal(Convert.ToUInt64(value)); }
                    else
                    { return SyntaxFactory.Literal(Convert.ToDecimal(value)); }
                case LiteralKind.Boolean:
                case LiteralKind.Type:
                // Need to create an expression so handled separately and should not call this
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public static SyntaxTriviaList BuildStructuredDocumentationSyntax(IHasStructuredDocumentation itemHasStructDoc)
        {
            var ret = SyntaxFactory.TriviaList();
            if (itemHasStructDoc == null || string.IsNullOrWhiteSpace(itemHasStructDoc.Description)) return ret;
            var description = "\r\n" + itemHasStructDoc.Description + "\r\n";
            XDocument xDoc = null;
            if (itemHasStructDoc.StructuredDocumentation == null)
            {
                xDoc = new XDocument();
            }
            else
            {
                xDoc = XDocument.Parse(itemHasStructDoc.StructuredDocumentation.Document);
            }
            var oldParent = xDoc.DescendantNodes().OfType<XElement>().Where(x => x.Name == "member").FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(description))
            {
                var oldSummaryElement = xDoc
                            .DescendantNodes().OfType<XElement>()
                            .Where(x => x.Name == "summary")
                            .FirstOrDefault();
                if (oldSummaryElement != null)
                { oldSummaryElement.Value = description; }
                else
                {
                    var newSummary = new XElement("summary", description);
                    oldParent.AddFirst(newSummary);
                }
            }
            // No doubt I'll feel dirty in the morning, but the manual alternative is awful
            var oldDocsAsString = xDoc.ToString();
            var lines = oldDocsAsString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var newDocAsString = ""; // these are short, so decided not to use StringBuilder
            foreach (var line in lines)
            {
                var useLine = line.Trim();
                if (useLine.StartsWith("<member") || useLine.StartsWith("</member")) continue;
                newDocAsString += "/// " + useLine + "\r\n";
            }
            var triviaList = SyntaxFactory.ParseLeadingTrivia(newDocAsString);
            if (triviaList.Any())
            { return triviaList; }
            return ret;
        }


        //        private static XmlTextSyntax MakeXmlDocumentationExterior()
        //        {
        //            return SyntaxFactory.XmlText()
        //                    .WithTextTokens(
        //                        SyntaxFactory.TokenList(
        //                            SyntaxFactory.XmlTextLiteral(
        //                                SyntaxFactory.TriviaList(
        //                                    SyntaxFactory.DocumentationCommentExterior(
        //                                        @"///")),
        //                                @" ",
        //                                @" ",
        //                                SyntaxFactory.TriviaList())));

        //        }

        //        private static XmlElementEndTagSyntax GetEndTag(string name)
        //        {
        //            return SyntaxFactory.XmlElementEndTag(
        //                     SyntaxFactory.XmlName(
        //                         SyntaxFactory.Identifier(
        //                             name)));
        //        }

        //        private static XmlElementStartTagSyntax GetStartTag(string name)
        //        {
        //            return SyntaxFactory.XmlElementStartTag(
        //                     SyntaxFactory.XmlName(
        //                         SyntaxFactory.Identifier(
        //                             name)));
        //        }

        //        private static SyntaxToken XmlTextLiteral(string content)
        //        {
        //            return SyntaxFactory.XmlTextLiteral(
        //                            SyntaxFactory.TriviaList(
        //                                SyntaxFactory.DocumentationCommentExterior(
        //                                    @"    ///")),
        //                            " " + content,
        //                            " " + content,
        //                            SyntaxFactory.TriviaList());
        //        }

        //        private static SyntaxToken XmlNewLine()
        //        {
        //            return SyntaxFactory.XmlTextNewLine(
        //                             SyntaxFactory.TriviaList(),
        //                             @"
        //",
        //                             @"
        //",
        //                             SyntaxFactory.TriviaList());
        //        }
        //        //private static XmlNodeSyntax MakeSummaryNode(string text)
        //        //{
        //        //    var element = SyntaxFactory.XmlElement(GetStartTag("summary"), GetEndTag("summary"));
        //        //    element = element.WithContent(
        //        //        SyntaxFactory.SingletonList<XmlNodeSyntax>(
        //        //            SyntaxFactory.XmlText()
        //        //            .WithTextTokens(
        //        //                SyntaxFactory.TokenList(
        //        //                    new[]{
        //        //                    XmlNewLine(),
        //        //                    XmlTextLiteral(text),
        //        //                    XmlNewLine(),
        //        //                    XmlTextLiteral("") }
        //        //                    ))));
        //        //    return element;
        //        //}

        public static SyntaxTokenList SyntaxTokensForAccessModifier(AccessModifier accessModifier)
        {
            var tokenList = SyntaxFactory.TokenList();
            switch (accessModifier)
            {
                case AccessModifier.None:
                    return tokenList;
                case AccessModifier.Private:
                    return tokenList.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                case AccessModifier.ProtectedOrInternal:
                    return tokenList.AddRange(new SyntaxToken[] { SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.InternalKeyword) });
                case AccessModifier.Protected:
                    return tokenList.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                case AccessModifier.Internal:
                    return tokenList.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                case AccessModifier.Public:
                    return tokenList.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                default:
                    throw new InvalidOperationException();
            }
        }
        //public static BlockSyntax BuildStatementBlock(this IEnumerable<IStatement> statements)
        //{
        //    var statementSyntaxList = new List<StatementSyntax>();
        //    foreach (var statement in statements)
        //    {
        //        //  statementSyntaxList.Add(RDomStatement statement.BuildSyntax());
        //    }
        //    //if (statementContainer.Statements.Count() == 0) { statements.Add(SyntaxFactory.EmptyStatement()); }
        //    var ret = SyntaxFactory.Block(statementSyntaxList);
        //    return ret;
        //}

        public static T AttachWhitespace<T>(T syntaxNode, Whitespace2Set whitespace2Set, WhitespaceKindLookup whitespaceLookup)
             where T : SyntaxNode
        {
            var ret = syntaxNode;
            foreach (var whitespace2 in whitespace2Set)
            {
                ret = AttachWhitespaceItem(ret, whitespace2, whitespaceLookup);
            }
            return ret;
        }

        private static T AttachWhitespaceItem<T>(T syntaxNode, Whitespace2 whitespace2, WhitespaceKindLookup whitespaceLookup)
             where T : SyntaxNode
        {
            var ret = syntaxNode;
            var kind = whitespaceLookup.Lookup(whitespace2.LanguageElement);
            var tokens = syntaxNode.ChildTokens().Where(x => x.CSharpKind() == kind);
            if (!tokens.Any() && whitespace2.LanguageElement == LanguageElement.Identifier)
            {
                var nameNode = syntaxNode.ChildNodes().OfType<NameSyntax>().FirstOrDefault();
                if (nameNode != null)
                { tokens = nameNode.DescendantTokens().Where(x => x.CSharpKind() == kind); }
            }
            // Sometimes the token won't be there due to changes in the tree. 
            if (tokens.Any())
            {
                var newToken = tokens.First();
                var leadingTrivia = SyntaxFactory.ParseLeadingTrivia(whitespace2.LeadingWhitespace)
                           .Concat(newToken.LeadingTrivia);
                var trailingTrivia = SyntaxFactory.ParseTrailingTrivia(whitespace2.TrailingWhitespace)
                           .Concat(newToken.TrailingTrivia);
                // Manage EOL comment here
                newToken = newToken
                            .WithLeadingTrivia(leadingTrivia)
                            .WithTrailingTrivia(trailingTrivia);
                ret = ret.ReplaceToken(tokens.First(), newToken);
            }
            return ret;
        }

        public static T AttachWhitespaceToFirst<T>(T syntaxNode, Whitespace2 whitespace2)
                where T : SyntaxNode
        {
            if (whitespace2 == null) { return syntaxNode; }
            var token = syntaxNode.GetFirstToken();
            var newToken = token;
            var ret = syntaxNode;
            var leadingTrivia = SyntaxFactory.ParseLeadingTrivia(whitespace2.LeadingWhitespace)
                       .Concat(newToken.LeadingTrivia);
            //var trailingTrivia = SyntaxFactory.ParseTrailingTrivia(whitespace2.TrailingWhitespace)
            //           .Concat(newToken.TrailingTrivia);
            // Manage EOL comment here
            //newToken = newToken
            //            .WithLeadingTrivia(leadingTrivia)
            //            .WithTrailingTrivia(trailingTrivia);
            newToken = newToken
                       .WithLeadingTrivia(leadingTrivia);
            ret = ret.ReplaceToken(token, newToken);
            return ret;
        }

        public static T AttachWhitespaceToLast<T>(T syntaxNode, Whitespace2 whitespace2)
                 where T : SyntaxNode
        {
            if (whitespace2 == null) { return syntaxNode; }
            var token = syntaxNode.GetLastToken();
            var newToken = token;
            var ret = syntaxNode;
            //var leadingTrivia = SyntaxFactory.ParseLeadingTrivia(whitespace2.LeadingWhitespace)
            //           .Concat(newToken.LeadingTrivia);
            var trailingTrivia = SyntaxFactory.ParseTrailingTrivia(whitespace2.TrailingWhitespace)
                       .Concat(newToken.TrailingTrivia);
            // Manage EOL comment here
            //newToken = newToken
            //            .WithLeadingTrivia(leadingTrivia)
            //            .WithTrailingTrivia(trailingTrivia);
            newToken = newToken
                        .WithTrailingTrivia(trailingTrivia);
            ret = ret.ReplaceToken(token, newToken);
            return ret;
        }

        public static T AttachWhitespaceToFirstAndLast<T>(T syntaxNode, Whitespace2 whitespace2)
         where T : SyntaxNode
        {
            if (whitespace2 == null) { return syntaxNode; }
            syntaxNode = AttachWhitespaceToFirst(syntaxNode, whitespace2);
            syntaxNode = AttachWhitespaceToLast(syntaxNode, whitespace2);
            return syntaxNode;
        }


    }
}
