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
   public class StructuredDocumentationFactory
               : RDomBaseSyntaxNodeFactory<RDomStructuredDocumentation, SyntaxNode>, ITriviaFactory<IStructuredDocumentation>
   {

      public StructuredDocumentationFactory(RDomCorporation corporation)
          : base(corporation)
      { }

      public override RDomPriority Priority
      { get { return 0; } }

      public override Type[] SpecialExplicitDomTypes
      { get { return new Type[] { typeof(IStructuredDocumentation) }; } }

      public RDomCorporation Corporation
      {
         get { return OutputContext.Corporation; }
         set
         {            // do nothing, already set in constructor 
         }
      }

      /// <summary>
      /// asdflkjdfg
      /// 
      /// asdfsdf
      /// </summary>
      /// <param name="model"></param>
      /// <returns></returns>
      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var newItem = new RDomStructuredDocumentation(syntaxNode, parent, model);
         IEnumerable<SyntaxTrivia> leadingTrivia = syntaxNode.GetLeadingTrivia();
         if (leadingTrivia
                        .Any(x => x.Kind() == SyntaxKind.MultiLineDocumentationCommentTrivia))
         { throw new NotImplementedException(); }
         if (leadingTrivia.Any(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia))
         {


            var leadingWs = GetLeadingWs(leadingTrivia);

            var structuredDocs = leadingTrivia
                     .Where(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);
            var docNode = structuredDocs.First().GetStructure() as DocumentationCommentTriviaSyntax;
            if (docNode == null) { throw new InvalidOperationException(); }

            var elements = ElementsFromTriviaNodes(newItem, model, docNode.ChildNodes().OfType<XmlNodeSyntax>());

            var newWs = new Whitespace2(LanguagePart.Current, LanguageElement.DocumentationComment);
            newWs.LeadingWhitespace = leadingWs;
            newItem.Whitespace2Set.Add(newWs);

            newItem.Elements.AddOrMoveRange(elements);

         }
         return newItem;
      }


      private static IEnumerable<RDomStructuredDocumentationElement> ElementsFromTriviaNodes(
                  RDomStructuredDocumentation parent,
                  SemanticModel model,
                  IEnumerable<XmlNodeSyntax> nodes)
      {
         var elements = new List<RDomStructuredDocumentationElement>();
         foreach (var node in nodes)
         {

            var text = node as XmlTextSyntax;
            if (text != null)
            {
               // Not currently storing, assume single space as these should interleave elements
            }
            else
            {
               var elementSyntax = node as XmlElementSyntax;
               if (elementSyntax != null)
               {
                  var element = new RDomStructuredDocumentationElement(node, parent, model);
                  element.Name = elementSyntax.StartTag.Name.ToString();
                  element.Text = TextFromSyntax(elementSyntax);
                  var attributes = AttributesFromSyntax(parent, model, elementSyntax.StartTag.Attributes);
                  element.Attributes.AddOrMoveAttributeRange(attributes);
                  elements.Add(element);
               }
               else
               { throw new NotImplementedException(); }
            }
         }
         return elements;
      }

      private static string TextFromSyntax(XmlElementSyntax elementSyntax)
      {
         var ret = "";
         foreach (var text in elementSyntax.ChildNodes().OfType<XmlTextSyntax>())
         {
            ret += TextFromTokens(text.ChildTokens());
         }
         return ret;
      }

      private static string TextFromTokens(IEnumerable<SyntaxToken> tokens)
      {
         var ret = "";
         foreach (var token in tokens)
         {
            if (token.Kind() == SyntaxKind.XmlTextLiteralNewLineToken)
            {
               ret += "\r\n";
            }
            else if (token.Kind() == SyntaxKind.XmlTextLiteralToken)
            {
               var value = token.ValueText;
               if (value.StartsWith(" ")) { value = value.Substring(1); }
               ret += value;
            }
            else
            {
               Logger.Log.InvalidXmlDocumentation(token.ToString(), "Unexpected Token Type");
            }
         }

         return ret;
      }

      private static IEnumerable<RDomAttribute> AttributesFromSyntax(
                     RDomStructuredDocumentation parent,
                     SemanticModel model,
                     IEnumerable<XmlAttributeSyntax> attributes)
      {
         var list = new List<RDomAttribute>();
         foreach (var attributeSyntax in attributes)
         {
            var valueNode = attributeSyntax
                           .ChildNodes()
                           .LastOrDefault();

            var attribute = new RDomAttribute(attributeSyntax, parent, model);
            attribute.Name = attributeSyntax.Name.ToString();

            var value = "";
            if (valueNode != null)
            {
               value = valueNode.ToString();
               var attributeValue = new RDomAttributeValue(attribute, "", value: value);
               attribute.AddOrMoveAttributeValue(attributeValue);
            }
            list.Add(attribute);
         }
         return list;
      }

      private string GetLeadingWs(IEnumerable<SyntaxTrivia> leadingTrivia)
      {
         var trivia = leadingTrivia
                         .Where(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia)
                         .First();
         var precedingTrivia = leadingTrivia.PreviousSiblings<SyntaxTrivia>(trivia)
                         .LastOrDefault();
         var leadingWs = "";
         if (precedingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
         { leadingWs = precedingTrivia.ToFullString(); }
         return leadingWs;
      }

      [ExcludeFromCodeCoverage]
      public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
      {
         return null;
      }

      public IDom CreateFrom(string possibleAnnotation, OutputContext context)
      {
         throw new NotImplementedException();
      }

      public IDom CreateFrom(SyntaxTrivia trivia, IDom parent, OutputContext context)
      {
         throw new NotImplementedException();
      }

      public IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IDom item, OutputContext context)
      {
         var triviaString = "";
         var ret = new List<SyntaxTrivia>();
         var itemStructDoc = item as IStructuredDocumentation;
         if (itemStructDoc == null) return ret;
         var leadingWs = "";
         var innerLeadingWs = " ";

         foreach (var element in itemStructDoc.Elements)
         {
            triviaString += GetElementString(element);
         }


         ret.AddRange(SyntaxFactory.ParseLeadingTrivia(triviaString));
         return ret;
      }

      private string GetElementString(IStructuredDocumentationElement element)
      {
         var attributes = GetAttributeString(element);
         var ret = $"{DocPrefix} <{element.Name}{attributes}>\r\n";
         var lines = element.Text.SplitLines().ToArray();
         for (int i = 0; i < lines.Length; i++)
         {
            var line = lines[i];
            if ((i > 0 && i < lines.Length - 1) || !string.IsNullOrWhiteSpace(line))
            {
               var nullSpace = string.IsNullOrWhiteSpace(line)
                                 ? ""
                                 : " ";
               ret += $"{DocPrefix}{nullSpace}{lines[i]}\r\n";
            }
         }
         ret += $"{DocPrefix} </{element.Name}>\r\n";
         return ret;
      }

      private object GetAttributeString(IStructuredDocumentationElement element)
      {
         var ret = "";
         foreach (var attribute in element.Attributes)
         {
            var values = string.Join(", ", attribute.AttributeValues.Select(x => x.Value));
            ret += $@" {attribute.Name}=""{values}""";
         }
         return ret;
      }

      private string DocPrefix => "   ///";

   }
}
