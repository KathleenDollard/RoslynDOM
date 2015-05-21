using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomStructuredDocumentationElement
      : RDomBase<IStructuredDocumentationElement, ISymbol>, IStructuredDocumentationElement, IHasSameIntentMethod
   {
      private AttributeCollection _attributes = new AttributeCollection();
      public RDomStructuredDocumentationElement(SyntaxNode rawItem, IDom parent, SemanticModel model)
            : base(rawItem, parent, model)
      { }

      public RDomStructuredDocumentationElement(RDomStructuredDocumentation oldRDom)
            : base(oldRDom)
      {
      }

      public string Text { get; set; }
      public string Name { get; set; }
      public AttributeCollection Attributes
      { get { return _attributes; } }
   }
}
