using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public static class RoslynCSharpUtilities
   {

      public static string NameFrom(this SyntaxNode node)
      {
         var qualifiedNameNode = node.ChildNodes()
                                   .OfType<QualifiedNameSyntax>()
                                   .SingleOrDefault();
         var identifierNameNodes = node.ChildNodes()
                            .OfType<IdentifierNameSyntax>()
                            .ToList();
         var name = "";
         if (qualifiedNameNode != null)
         {
            name = name + qualifiedNameNode.ToString();
         }
         foreach (var identifierNameNode in identifierNameNodes)
         {
            var identifierName = identifierNameNode.ToString();
            if (!(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(identifierName)))
            { name += "."; }
            name = name += identifierName;
         }
         if (!string.IsNullOrWhiteSpace(name)) return name;
         var nameToken = node.ChildTokens()
                                   .Where(x => x.Kind() == SyntaxKind.IdentifierToken)
                                   .SingleOrDefault();
         return nameToken.ValueText;
      }

      public static StatementSyntax BuildStatement(IEnumerable<IStatement> statements,
           IStatementBlock parent, WhitespaceKindLookup whitespaceLookup)
      {
         StatementSyntax statementBlock;
         var statementSyntaxList = statements
                      .SelectMany(x => RDom.CSharp.GetSyntaxGroup(x))
                      .ToList();
         var hasBlock = parent.HasBlock;
         if (hasBlock || statements.Count() > 1)
         {
            statementBlock = SyntaxFactory.Block(SyntaxFactory.List(statementSyntaxList));
            statementBlock = BuildSyntaxHelpers.AttachWhitespace(statementBlock, parent.Whitespace2Set, whitespaceLookup);
            // Block tokens are held in parent
         }
         else if (statements.Count() == 1)
         {
            statementBlock = (StatementSyntax)statementSyntaxList.First();
            //statementBlock = BuildSyntaxHelpers.AttachWhitespace(statementBlock, parent.Whitespace2Set, whitespaceLookup);
         }
         else
         {
            statementBlock = SyntaxFactory.EmptyStatement();
            statementBlock = BuildSyntaxHelpers.AttachWhitespace(statementBlock, parent.Whitespace2Set, whitespaceLookup);
         }
         return statementBlock;
      }

      //public static RDomStructuredDocumentation DocumentFromDocComment(
      //         DocumentationCommentTriviaSyntax trivia)
      //{
      //   var elements = ElementsFromTriviaNodes(
      //               trivia.ChildNodes().OfType<XmlNodeSyntax>());
      //   var doc = new RDomStructuredDocumentation()
      //   return new XDocument(xElement);
      //}

      //private static IEnumerable<RDomStructuredDocumentationElement> ElementsFromTriviaNodes(
      //            IEnumerable<XmlNodeSyntax> nodes,
      //            IEnumerable<XmlAttributeSyntax> attributes = null)
      //{
      //   var xElement = new XElement(parentTag);
      //   foreach (var node in nodes)
      //   {
      //      var text = node as XmlTextSyntax;
      //      if (text != null)
      //      {
      //         // Not currently storing, assume single space as these should interleave elements
      //      }
      //      else
      //      {
      //         var element = node as XmlElementSyntax;
      //         if (element != null)
      //         {
      //            xElement.Add(XElementFromElementSyntax(element.StartTag.Name.ToFullString(),
      //                        node.ChildNodes().OfType<XmlNodeSyntax>(),
      //                        element.StartTag.Attributes));
      //         }
      //         else
      //         { throw new NotImplementedException(); }
      //      }
      //   }
      //   return xElement;
      //}

      //private static XElement XElementFromElementSyntax(string parentTag,
      //       IEnumerable<XmlNodeSyntax> nodes,
      //       IEnumerable<XmlAttributeSyntax> attributes = null)
      //{
      //   var xElement = new XElement(parentTag);
      //   foreach (var node in nodes)
      //   {
      //      var text = node as XmlTextSyntax;
      //      if (text != null)
      //      {
      //         xElement.Add(XTextFromComments(text));
      //      }
      //      else
      //      {
      //         var element = node as XmlElementSyntax;
      //         if (element != null)
      //         {
      //            // Not currently storing, assume no nesting
      //         }
      //         else
      //         { throw new NotImplementedException(); }
      //      }
      //   }
      //   if (attributes != null)
      //   { AddAttributes(xElement, attributes); }
      //   return xElement;
      //}

      //private static void AddAttributes(XElement xElement, IEnumerable<XmlAttributeSyntax> attributes)
      //{
      //   foreach (var attribute in attributes)
      //   {
      //      var valueNode = attribute
      //                     .ChildNodes()
      //                     .LastOrDefault();
      //      var value = "";
      //      if (valueNode != null)
      //      { value = valueNode.ToString(); }
      //      var xAttribute = new XAttribute(attribute.Name.ToString(), value);
      //      xElement.Add(xAttribute);
      //   }
      //}

      //private static IEnumerable<XText> XTextFromComments(XmlTextSyntax text)
      //{
      //   var list = new List<XText>();
      //   var tokens = text.ChildTokens();
      //   var currentLine = "";
      //   foreach (var token in tokens)
      //   {
      //      if (token.Kind() == SyntaxKind.XmlTextLiteralNewLineToken)
      //      {
      //         list.Add(new XText(currentLine));
      //         currentLine = "";
      //      }
      //      else if (token.Kind() == SyntaxKind.XmlTextLiteralToken)
      //      {
      //         currentLine = token.ValueText;
      //         if (currentLine.StartsWith(" ")) { currentLine = currentLine.Substring(1); }
      //      }
      //      else
      //      {
      //         Logger.Log.InvalidXmlDocumentation(token.ToString(), "Unexpected Token Type");
      //      }
      //   }
      //   if (!string.IsNullOrEmpty(currentLine))
      //   {
      //      var newText = new XText(currentLine);
      //      if (tokens.Count() >= 1) { newText.AddAnnotation(new SkipFirstLine()); }
      //      list.Add(newText);
      //   }
      //   return list;
      //}
      //public class SkipFirstLine
      //{ }

      //private static IEnumerable<XText> XTextFromComments(string comment)
      //{
      //   var ret = new List<XText>();
      //   var lines = comment.SplitLines().Where(x => !string.IsNullOrEmpty(x));
      //   foreach (var line in lines)
      //   {
      //      var temp = line;
      //      temp = line.TrimStart();
      //      if (temp.StartsWith("///")) { temp = temp.Substring(3); }
      //      if (temp.StartsWith(" ")) { temp = temp.Substring(1); }
      //      ret.Add(new XText(temp));
      //   }
      //   return ret;
      //}

      //public static DocumentationCommentTriviaSyntax DocCommentFromXDocument(XDocument xDoc)
      //{
      //   var root = xDoc.Nodes().OfType<XElement>().FirstOrDefault();
      //   if (root == null) { return null; }
      //   var list = GetXmlDocElements(root.Nodes());

      //   var content = SyntaxFactory.List<XmlNodeSyntax>(list);
      //   var trivia = SyntaxFactory.DocumentationCommentTrivia(
      //            SyntaxKind.SingleLineDocumentationCommentTrivia,
      //            content);
      //   return trivia;
      //}

      //private static List<XmlNodeSyntax> GetXmlDocElements(IEnumerable<XNode> nodes)
      //{
      //   var list = new List<XmlNodeSyntax>();
      //   foreach (var node in nodes)
      //   {
      //      var xText = node as XText;
      //      if (xText != null)
      //      {
      //         // Not currently used
      //      }
      //      else
      //      {
      //         var xElement = node as XElement;
      //         if (xElement != null)
      //         {
      //            list.Add(GetXmlDocElement(xElement));
      //         }
      //         else { throw new NotImplementedException(); }
      //      }
      //   }
      //   return list;
      //}

      //private static XmlNodeSyntax GetXmlDocElement(XElement xElement)
      //{
      //   var tagName = SyntaxFactory.XmlName(xElement.Name.LocalName);
      //   var startTag = SyntaxFactory.XmlElementStartTag(tagName)
      //                     .WithAttributes(GetXmlAttributesFromElement(xElement));
      //   var endTag = SyntaxFactory.XmlElementEndTag(tagName);
      //   // not currently supporting nested patterns of XElements
      //   var tokenList = new List<SyntaxToken>();
      //   foreach (var xTextChild in xElement.Nodes().OfType<XText>())
      //   {
      //      tokenList.AddRange(GetXmlTextFromChildNode(xTextChild));
      //   }
      //   var textSyntax = SyntaxFactory.XmlText(SyntaxFactory.TokenList(tokenList));
      //   var content = SyntaxFactory.List(new List<SyntaxNode>() { textSyntax });
      //   var element = SyntaxFactory.XmlElement(startTag, content, endTag);
      //   return element;
      //}

      //private static SyntaxList<XmlAttributeSyntax> GetXmlAttributesFromElement(XElement xElement)
      //{
      //   var list = SyntaxFactory.List<XmlAttributeSyntax>();
      //   foreach (var attribute in xElement.Attributes())
      //   {
      //      var name = SyntaxFactory.XmlName(attribute.Name.LocalName);
      //      var startQuoteToken = SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken);
      //      var endQuoteToken = SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken);
      //      var token = SyntaxFactory.Token(SyntaxFactory.TriviaList(),
      //        SyntaxKind.XmlTextLiteralToken, attribute.Value, attribute.Value, SyntaxFactory.TriviaList());
      //      var tokens = SyntaxFactory.TokenList(token);
      //      var attributeSyntax = SyntaxFactory.XmlTextAttribute(name, startQuoteToken, tokens, endQuoteToken);
      //      list = list.Add(attributeSyntax);
      //   }
      //   return list;
      //}

      //private static SyntaxTrivia docTrivia = SyntaxFactory.DocumentationCommentExterior("///");

      //private static IEnumerable<SyntaxToken> GetXmlTextFromChildNode(XText xText)
      //{
      //   var tokenList = new List<SyntaxToken>();

      //   tokenList.Add(MakeNewLineToken());
      //   tokenList.Add(MakeLiteralToken(xText.Value));
      //   return tokenList;
      //}

      //private static SyntaxToken MakeNewLineToken()
      //{
      //   var token = SyntaxFactory.Token(
      //         SyntaxFactory.TriviaList(),
      //         SyntaxKind.XmlTextLiteralNewLineToken,
      //         SyntaxFactory.TriviaList());
      //   return token;
      //}

      //private static SyntaxToken MakeLiteralToken(string s)
      //{
      //   if (!string.IsNullOrEmpty(s)) { s = " " + s; }
      //   var token = SyntaxFactory.Token(
      //         SyntaxFactory.TriviaList(docTrivia),
      //         SyntaxKind.XmlTextLiteralToken, s, s,
      //         SyntaxFactory.TriviaList());
      //   return token;
      //}
   }
}
