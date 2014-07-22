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
            var xmlComments = BuildSyntaxExtensions.BuildStructuredDocumentationSyntax(item as IHasStructuredDocumentation);
            if (xmlComments != null) leadingTrivia.Add(SyntaxFactory.Trivia(xmlComments));
            return SyntaxFactory.TriviaList(leadingTrivia);
        }

        public static DocumentationCommentTriviaSyntax BuildStructuredDocumentationSyntax(IHasStructuredDocumentation hasStructuredDocumentation)
        {

            if (hasStructuredDocumentation == null) return null;
            var description = hasStructuredDocumentation.Description;
            XDocument xDoc = null;
            if (hasStructuredDocumentation.StructuredDocumentation == null)
            {
                xDoc = new XDocument();
            }
            else
            {
                xDoc = hasStructuredDocumentation.StructuredDocumentation.RawItem as XDocument;
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
            var lines = oldDocsAsString.Split(new string[] { "\r\n"}, StringSplitOptions.None);
            var newDocAsString = ""; // these are short, so decided not to use StringBuilder
            foreach (var line in lines)
            {
                var useLine =  line.Trim();
                if (useLine.StartsWith("<member") || useLine.StartsWith("</member")) continue;
                newDocAsString += "/// " + useLine + "\r\n";
            }
            var triviaList = SyntaxFactory.ParseLeadingTrivia(newDocAsString);
            //if (triviaList.Any())
            //{ return (DocumentationCommentTriviaSyntax)((object)triviaList.First()); }
            return null;
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
            //f (statementContainer.Statements.Count() == 0) { statements.Add(SyntaxFactory.EmptyStatement()); }
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
