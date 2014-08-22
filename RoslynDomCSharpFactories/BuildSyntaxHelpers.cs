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
        // it doesn't feel like this belongs here, but until the design of PrepareForBuildSyntaxOutput solidifies, leaving it
        private static TriviaManager triviaManager = new TriviaManager();

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


        private class RDomWhitespaceHandledAnnotation
        {
            public const string Kind = "RDomWhitespaceHandled";
            public static SyntaxAnnotation Create()
            { return new SyntaxAnnotation(Kind); }
            //public static Guid GetMarker(SyntaxAnnotation annotation)
            //{ return Guid.Parse(annotation.Data); }
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
                    var innerWs = itemAsComment.Whitespace2Set[LanguagePart.Inner, LanguageElement.Comment];
                    var comment = innerWs.LeadingWhitespace + itemAsComment.Text + innerWs.TrailingWhitespace;
                    if (itemAsComment.IsMultiline) { comment = "/*" + comment + "*/"; }
                    else { comment = "//" + comment; }
                    var commentSyntax = SyntaxFactory.Comment(comment);

                    // Assume just one whitespace
                    var whitespace = itemAsComment.Whitespace2Set.FirstOrDefault();
                    if (whitespace != null)
                    {
                        // for now assume only whitespace before and newline after
                        ret.Add(SyntaxFactory.Whitespace(whitespace.LeadingWhitespace));
                        ret.Add(commentSyntax);
                    }
                    else
                    { ret.Add(commentSyntax); }
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

        public static IEnumerable<SyntaxTrivia> BuildStructuredDocumentationSyntax(IHasStructuredDocumentation itemHasStructDoc)
        {
            var ret = new List<SyntaxTrivia>();
            if (itemHasStructDoc == null ||
                (itemHasStructDoc.StructuredDocumentation.Document  == null
                && string.IsNullOrEmpty(itemHasStructDoc.Description)))
            { return ret; }
            var itemStructDoc = itemHasStructDoc.StructuredDocumentation;
            var leadingWs = "";
            var innerLeadingWs = " ";
            XDocument xDoc = null;

            if (itemHasStructDoc.StructuredDocumentation == null)
            {
                xDoc = new XDocument();
            }
            else
            {
                xDoc = XDocument.Parse(itemHasStructDoc.StructuredDocumentation.Document);
                leadingWs = itemStructDoc.Whitespace2Set[LanguageElement.DocumentationComment].LeadingWhitespace;
                innerLeadingWs = itemStructDoc.Whitespace2Set[LanguagePart.Inner, LanguageElement.DocumentationComment].LeadingWhitespace;
            }
            var description = "\r\n" + innerLeadingWs + itemHasStructDoc.Description + "\r\n";

            SetDescriptionInXDoc(xDoc, description);
            string newDocAsString = PrefixLinesWithDocCommentPrefix(leadingWs, xDoc);

            var triviaList = SyntaxFactory.ParseLeadingTrivia(newDocAsString);
            if (triviaList.Any())
            { return triviaList; }
            return ret;
        }

        private static void SetDescriptionInXDoc(XDocument xDoc, string description)
        {
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
        }

        private static string PrefixLinesWithDocCommentPrefix(string leadingWs, XDocument xDoc)
        {
            // No doubt I'll feel dirty in the morning, but the manual alternative is awful
            var oldDocsAsString = xDoc.ToString();
            var lines = oldDocsAsString.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var newDocAsString = ""; // these are short, so decided not to use StringBuilder
            foreach (var line in lines)
            {
                var useLine = line.Trim();
                if (useLine.StartsWith("<member") || useLine.StartsWith("</member")) continue;
                newDocAsString += leadingWs + "/// " + useLine + "\r\n";
            }

            return newDocAsString;
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

        //public static T AttachWhitespace<T>(T syntaxNode, Whitespace2Set whitespace2Set, WhitespaceKindLookup whitespaceLookup)
        //     where T : SyntaxNode
        //{
        //    var ret = syntaxNode;
        //    foreach (var whitespace2 in whitespace2Set)
        //    {
        //        ret = AttachWhitespaceItem(ret, whitespace2, whitespaceLookup);
        //    }
        //    return ret;
        //}

        //private static T AttachWhitespaceItem<T>(T syntaxNode, Whitespace2 whitespace2, WhitespaceKindLookup whitespaceLookup)
        //     where T : SyntaxNode
        //{
        //    var ret = syntaxNode;
        //    var kind = whitespaceLookup.Lookup(whitespace2.LanguageElement);
        //    var tokens = syntaxNode.ChildTokens().Where(x => x.CSharpKind() == kind);
        //    if (!tokens.Any() && whitespace2.LanguageElement == LanguageElement.Identifier)
        //    {
        //        var nameNode = syntaxNode.ChildNodes().OfType<NameSyntax>().FirstOrDefault();
        //        if (nameNode != null)
        //        { tokens = nameNode.DescendantTokens().Where(x => x.CSharpKind() == kind); }
        //    }
        //    // Sometimes the token won't be there due to changes in the tree. 
        //    if (tokens.Any())
        //    {
        //        var newToken = tokens.First();
        //        var leadingTrivia = SyntaxFactory.ParseLeadingTrivia(whitespace2.LeadingWhitespace)
        //                   .Concat(newToken.LeadingTrivia);
        //        var trailingTrivia = SyntaxFactory.ParseTrailingTrivia(whitespace2.TrailingWhitespace)
        //                   .Concat(newToken.TrailingTrivia);
        //        // Manage EOL comment here
        //        newToken = newToken
        //                    .WithLeadingTrivia(leadingTrivia)
        //                    .WithTrailingTrivia(trailingTrivia);
        //        ret = ret.ReplaceToken(tokens.First(), newToken);
        //    }
        //    return ret;
        //}

        //public static T AttachWhitespaceToFirst<T>(T syntaxNode, Whitespace2 whitespace2)
        //        where T : SyntaxNode
        //{
        //    if (whitespace2 == null) { return syntaxNode; }
        //    var token = syntaxNode.GetFirstToken();
        //    var newToken = token;
        //    var ret = syntaxNode;
        //    var leadingTrivia = SyntaxFactory.ParseLeadingTrivia(whitespace2.LeadingWhitespace)
        //               .Concat(newToken.LeadingTrivia);
        //    //var trailingTrivia = SyntaxFactory.ParseTrailingTrivia(whitespace2.TrailingWhitespace)
        //    //           .Concat(newToken.TrailingTrivia);
        //    // Manage EOL comment here
        //    //newToken = newToken
        //    //            .WithLeadingTrivia(leadingTrivia)
        //    //            .WithTrailingTrivia(trailingTrivia);
        //    newToken = newToken
        //               .WithLeadingTrivia(leadingTrivia);
        //    ret = ret.ReplaceToken(token, newToken);
        //    return ret;
        //}

        //public static T AttachWhitespaceToLast<T>(T syntaxNode, Whitespace2 whitespace2)
        //         where T : SyntaxNode
        //{
        //    if (whitespace2 == null) { return syntaxNode; }
        //    var token = syntaxNode.GetLastToken();
        //    var newToken = token;
        //    var ret = syntaxNode;
        //    //var leadingTrivia = SyntaxFactory.ParseLeadingTrivia(whitespace2.LeadingWhitespace)
        //    //           .Concat(newToken.LeadingTrivia);
        //    var trailingTrivia = SyntaxFactory.ParseTrailingTrivia(whitespace2.TrailingWhitespace)
        //               .Concat(newToken.TrailingTrivia);
        //    // Manage EOL comment here
        //    //newToken = newToken
        //    //            .WithLeadingTrivia(leadingTrivia)
        //    //            .WithTrailingTrivia(trailingTrivia);
        //    newToken = newToken
        //                .WithTrailingTrivia(trailingTrivia);
        //    ret = ret.ReplaceToken(token, newToken);
        //    return ret;
        //}

        //public static T AttachWhitespaceToFirstAndLast<T>(T syntaxNode, Whitespace2 whitespace2)
        // where T : SyntaxNode
        //{
        //    if (whitespace2 == null) { return syntaxNode; }
        //    syntaxNode = AttachWhitespaceToFirst(syntaxNode, whitespace2);
        //    syntaxNode = AttachWhitespaceToLast(syntaxNode, whitespace2);
        //    return syntaxNode;
        //}


        public static T AttachWhitespace<T>(T syntaxNode, Whitespace2Set whitespace2Set, WhitespaceKindLookup whitespaceLookup)
               where T : SyntaxNode
        {
            return triviaManager.AttachWhitespace(syntaxNode, whitespace2Set, whitespaceLookup);
        }

        public static T AttachWhitespace<T>(T syntaxNode, Whitespace2Set whitespace2Set, WhitespaceKindLookup whitespaceLookup, LanguagePart languagePart)
               where T : SyntaxNode
        {
            return triviaManager.AttachWhitespace(syntaxNode, whitespace2Set, whitespaceLookup, languagePart );
        }

        public static T AttachWhitespaceToFirst<T>(T syntaxNode, Whitespace2 whitespace2)
            where T : SyntaxNode
        {
            return triviaManager.AttachWhitespaceToFirst(syntaxNode, whitespace2);
        }

        public static T AttachWhitespaceToLast<T>(T syntaxNode, Whitespace2 whitespace2)
                 where T : SyntaxNode
        {
            return triviaManager.AttachWhitespaceToLast(syntaxNode, whitespace2);
        }

        public static T AttachWhitespaceToFirstAndLast<T>(T syntaxNode, Whitespace2 whitespace2)
         where T : SyntaxNode
        {
            return triviaManager.AttachWhitespaceToFirstAndLast(syntaxNode, whitespace2);
        }
        internal static SyntaxToken AttachWhitespaceToToken(SyntaxToken token, Whitespace2 whitespace2)
        {
            return triviaManager.AttachWhitespaceToToken(token, whitespace2);
        }
    }
}
