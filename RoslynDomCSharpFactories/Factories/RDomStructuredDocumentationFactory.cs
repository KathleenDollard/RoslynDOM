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
                : RDomMiscFactory<IStructuredDocumentation, SyntaxNode>
    {

        public StructuredDocumentationFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        public override RDomPriority Priority
        { get { return 0; } }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            // Always tries
            return true;
        }

        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var newItem = new RDomStructuredDocumentation(syntaxNode, parent, model);
            var parentAsHasSymbol = parent as IRoslynHasSymbol;
            if (parentAsHasSymbol != null)
            {
                var symbol = parentAsHasSymbol.Symbol;
                var docString = symbol.GetDocumentationCommentXml();
                if (!string.IsNullOrEmpty(docString))
                {
                    var xDocument = XDocument.Parse(docString);
                    var summaryNode = xDocument.DescendantNodes()
                                        .OfType<XElement>()
                                        .Where(x => x.Name == "summary")
                                        .Select(x => x.Value);
                    var description = summaryNode.FirstOrDefault().Replace("/r", "").Replace("\n", "").Trim();
                    newItem.Description = description;
                    newItem.Document = docString;
                }
            }

            return newItem;
        }

        [ExcludeFromCodeCoverage]
        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            return null;
        }

    }
}
