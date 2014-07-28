using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomStructuredDocumentation : RDomBase<IStructuredDocumentation, ISymbol>, IStructuredDocumentation, IHasSameIntentMethod
    {
        public RDomStructuredDocumentation(SyntaxNode rawItem, IDom parent, SemanticModel model)
            : base(rawItem, parent, model )
        {     }

           public RDomStructuredDocumentation(RDomStructuredDocumentation oldRDom)
            :  base(oldRDom)
        { }
        public string Description { get; set; }
        public string Document { get; set; }

    }
}
