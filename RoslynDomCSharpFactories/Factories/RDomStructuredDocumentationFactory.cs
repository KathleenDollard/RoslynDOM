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

      protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         var newItem = new RDomStructuredDocumentation(syntaxNode, parent, model);
         var parentAsHasSymbol = parent as IRoslynHasSymbol;
         if (parentAsHasSymbol != null)
         {
            var symbol = parentAsHasSymbol.Symbol;
            if (symbol != null)
            {
               var docString = symbol.GetDocumentationCommentXml();
               if (!string.IsNullOrEmpty(docString))
               {
                  IEnumerable<SyntaxTrivia> leadingTrivia = syntaxNode.GetLeadingTrivia();
                  if (leadingTrivia
                                  .Any(x => x.Kind() == SyntaxKind.MultiLineDocumentationCommentTrivia))
                  { throw new NotImplementedException(); }

                  var trivia = leadingTrivia
                                  .Where(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia)
                                  .First();
                  var precedingTrivia = leadingTrivia.PreviousSiblings<SyntaxTrivia>(trivia)
                                  .LastOrDefault();
                  var leadingWs = "";
                  if (precedingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                  { leadingWs = precedingTrivia.ToFullString(); }
                  var xDocument = XDocument.Parse(docString);
                  var summaryNode = xDocument.DescendantNodes()
                                      .OfType<XElement>()
                                      .Where(x => x.Name == "summary")
                                      .Select(x => x.Value);
                  var newWs = new Whitespace2(LanguagePart.Current, LanguageElement.DocumentationComment);
                  newWs.LeadingWhitespace = leadingWs;
                  newItem.Whitespace2Set.Add(newWs);

                  //var description = summaryNode.FirstOrDefault().Replace("\r", "").Replace("\n", "");
                  var description = summaryNode.FirstOrDefault();
                  leadingWs = description.SubstringBefore(description.Trim());
                  newWs = new Whitespace2(LanguagePart.Inner, LanguageElement.DocumentationComment);
                  newWs.LeadingWhitespace = leadingWs;
                  newItem.Whitespace2Set.Add(newWs);

                  newItem.Description = description.Trim();
                  newItem.Document = docString;
               }
            }
         }

         return newItem;
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
         var ret = new List<SyntaxTrivia>();
         var itemStructDoc = item as IStructuredDocumentation;
         if (itemStructDoc == null) return ret;
         var leadingWs = "";
         var innerLeadingWs = " ";
         XDocument xDoc = null;

         if (itemStructDoc.Document == null)
         {
            xDoc = new XDocument();
         }
         else
         {
            xDoc = XDocument.Parse(itemStructDoc.Document);
            leadingWs = itemStructDoc.Whitespace2Set[LanguageElement.DocumentationComment].LeadingWhitespace;
            innerLeadingWs = itemStructDoc.Whitespace2Set[LanguagePart.Inner, LanguageElement.DocumentationComment].LeadingWhitespace;
         }
         if (!(string.IsNullOrWhiteSpace(itemStructDoc.Description)))
         {
            var description = innerLeadingWs + itemStructDoc.Description + "\r\n";
            SetDescriptionInXDoc(xDoc, description);
         }
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
         if (string.IsNullOrWhiteSpace(oldDocsAsString)) return "";
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
   }
}
