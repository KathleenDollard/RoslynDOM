using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public static class BuildSyntaxExtensions
    {
        public static IEnumerable<SyntaxNode> PrepareForBuildSyntaxOutput(this IDom item, SyntaxNode node)
        {
            var leadingTrivia = BuildSyntaxExtensions.LeadingTrivia(item);
            node = node.WithLeadingTrivia(leadingTrivia);

            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }

        public static SyntaxList<AttributeListSyntax> WrapInAttributeList(this IEnumerable<SyntaxNode> attributes)
        {
            var node = SyntaxFactory.List<AttributeListSyntax>(attributes.OfType<AttributeListSyntax>());
            return node;
        }

        public static SyntaxTokenList BuildModfierSyntax(this IHasAccessModifier hasAccessModifier)
        {
            var list = SyntaxFactory.TokenList();
            if (hasAccessModifier != null)
            { list = list.AddRange(SyntaxTokensForAccessModifier(hasAccessModifier.AccessModifier)); }
            // TODO: Static and other modifiers
            return list;
        }

        public static SyntaxTriviaList LeadingTrivia(IDom item)
        {
            var leadingTrivia = new List<SyntaxTrivia>();

            leadingTrivia.AddRange(BuildSyntaxExtensions.BuildStructuredDocumentationSyntax(item as IHasStructuredDocumentation));

            leadingTrivia.AddRange(BuildCommentWhite(item));

            return SyntaxFactory.TriviaList(leadingTrivia);
        }

        //private static IEnumerable<SyntaxTrivia> BuildCommentWhite(IDom item)
        //{
        //    var ret = new List<SyntaxTrivia>();
        //    // This can happen if someone copies an item to a new item, does not attach it to a tree, 
        //    // and asks for the syntax. It's actually expected to sometimes be unattached. 
        //    if (item.Parent == null) { return ret; }
        //    if (item is IStemMember)
        //    {
        //        var parentAsStem = item.Parent as IStemContainer;
        //        if (parentAsStem == null) throw new InvalidOperationException();
        //        var commentWhites = parentAsStem.StemMembersAll
        //                            .PreviousSiblingsUntil(item, x => !(x is IComment || x is IVerticalWhitespace))
        //                            .OfType<ICommentWhite>();
        //        ret.AddRange(MakeWhiteCommentTrivia(commentWhites));
        //    }
        //    return ret;
        //}

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
                    if (itemAsComment == null) throw new InvalidOperationException();
                    var comment = "";
                    if (itemAsComment.IsMultiline) { comment = "/* " + itemAsComment.Text + "*/"; }
                    else { comment = "// " + itemAsComment.Text; }
                    ret.Add(SyntaxFactory.Comment(comment));
                    ret.Add(SyntaxFactory.EndOfLine("\r\n"));
                }
            }
            return ret;
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
                xDoc = itemHasStructDoc.StructuredDocumentation.RawItem as XDocument;
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

        //private static IEnumerable<XmlNodeSyntax> GetXmlNodes(IHasStructuredDocumentation item, string description)
        //{
        //    var xmlNodes = new List<XmlNodeSyntax>();
        //    if (item.StructuredDocumentation == null) return xmlNodes;
        //    var oldDocumentation = item.StructuredDocumentation.RawItem as XDocument;
        //    if (oldDocumentation != null)
        //    {
        //        var oldParent = oldDocumentation.DescendantNodes().OfType<XElement>().Where(x => x.Name == "member").FirstOrDefault();
        //        var oldSummaryElement = oldDocumentation
        //                    .DescendantNodes().OfType<XElement>()
        //                    .Where(x => x.Name == "summary")
        //                    .FirstOrDefault();
        //        if (oldSummaryElement != null)
        //        { oldSummaryElement.Value = description; }
        //        else
        //        {
        //            var newSummary = new XElement("summary", description);
        //            oldParent.AddFirst(newSummary);
        //        }
        //        // No doubt I'll feel dirty in the morning, but the manual alternative is awful
        //        var oldDocsAsString = oldDocumentation.ToString();
        //        var docNodes = SyntaxFactory.ParseLeadingTrivia(oldDocsAsString);
        //    }
        //    return docNodes;
        //}

        private static XmlTextSyntax MakeXmlDocumentationExterior()
        {
            return SyntaxFactory.XmlText()
                    .WithTextTokens(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.XmlTextLiteral(
                                SyntaxFactory.TriviaList(
                                    SyntaxFactory.DocumentationCommentExterior(
                                        @"///")),
                                @" ",
                                @" ",
                                SyntaxFactory.TriviaList())));

        }

        private static XmlElementEndTagSyntax GetEndTag(string name)
        {
            return SyntaxFactory.XmlElementEndTag(
                     SyntaxFactory.XmlName(
                         SyntaxFactory.Identifier(
                             name)));
        }

        private static XmlElementStartTagSyntax GetStartTag(string name)
        {
            return SyntaxFactory.XmlElementStartTag(
                     SyntaxFactory.XmlName(
                         SyntaxFactory.Identifier(
                             name)));
        }

        private static SyntaxToken XmlTextLiteral(string content)
        {
            return SyntaxFactory.XmlTextLiteral(
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.DocumentationCommentExterior(
                                    @"    ///")),
                            " " + content,
                            " " + content,
                            SyntaxFactory.TriviaList());
        }

        private static SyntaxToken XmlNewLine()
        {
            return SyntaxFactory.XmlTextNewLine(
                             SyntaxFactory.TriviaList(),
                             @"
",
                             @"
",
                             SyntaxFactory.TriviaList());
        }
        private static XmlNodeSyntax MakeSummaryNode(string text)
        {
            var element = SyntaxFactory.XmlElement(GetStartTag("summary"), GetEndTag("summary"));
            element = element.WithContent(
                SyntaxFactory.SingletonList<XmlNodeSyntax>(
                    SyntaxFactory.XmlText()
                    .WithTextTokens(
                        SyntaxFactory.TokenList(
                            new[]{
                            XmlNewLine(),
                            XmlTextLiteral(text),
                            XmlNewLine(),
                            XmlTextLiteral("") }
                            ))));
            return element;
        }

        public static SyntaxTokenList SyntaxTokensForAccessModifier(AccessModifier accessModifier)
        {
            var tokenList = SyntaxFactory.TokenList();
            switch (accessModifier)
            {
                case AccessModifier.NotApplicable:
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
        public static BlockSyntax BuildStatementBlock(this IEnumerable<IStatement> statements)
        {
            var statementSyntaxList = new List<StatementSyntax>();
            foreach (var statement in statements)
            {
                //  statementSyntaxList.Add(RDomStatement statement.BuildSyntax());
            }
            //if (statementContainer.Statements.Count() == 0) { statements.Add(SyntaxFactory.EmptyStatement()); }
            var ret = SyntaxFactory.Block(statementSyntaxList);
            return ret;
        }
        //public static SyntaxList<AttributeListSyntax> BuildAttributeListSyntax(IEnumerable<IAttribute> attributes)
        //{
        //    var list = SyntaxFactory.List<AttributeListSyntax>();
        //    if (attributes.Any())
        //    {
        //        var attribList = SyntaxFactory.AttributeList();
        //        var attributeSyntax = attributes.Select(x => ((RDomAttribute)x).BuildSyntax());
        //        var attributeSyntax = attributes.Select(x => .BuildSyntax());
        //        attribList = attribList.AddAttributes(attributeSyntax.ToArray());
        //        list = list.Add(attribList);
        //    }
        //    return list;
        //}

    }
}
