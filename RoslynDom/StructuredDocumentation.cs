using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomStructuredDocumentation : RDomBase<IStructuredDocumentation, ISymbol>, IStructuredDocumentation, IHasSameIntentMethod
   {

      private RDomCollection<IStructuredDocumentationElement> _elements;

      public RDomStructuredDocumentation(SyntaxNode rawItem, IDom parent, SemanticModel model)
            : base(rawItem, parent, model)
      { Initialize(); }


      public RDomStructuredDocumentation(RDomStructuredDocumentation oldRDom)
            : base(oldRDom)
      {
         Initialize();
         RDomCollection<IStructuredDocumentationElement>.Copy(oldRDom.Elements, _elements);
      }

      private void Initialize()
      {
         _elements = new RDomCollection<IStructuredDocumentationElement>(this);
      }

      public RDomCollection<IStructuredDocumentationElement> Elements
      { get { return _elements; } }

      //public string Description
      //{
      //   get
      //   {

      //      return DescriptionLines == null
      //            ? null
      //            : string.Join("\r\n", DescriptionLines.ToArray());
      //   }
      //}
      //public List<string> DescriptionLines { get; set; }
      //public string Document { get; set; }

      //public List<string> DocumentLines
      //{
      //   get
      //   {
      //      throw new NotImplementedException();
      //   }

      //   set
      //   {
      //      throw new NotImplementedException();
      //   }
      //}
   }
}
