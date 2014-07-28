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

        //public RDomStructuredDocumentation(object rawDocumentation, string description)
        //{
        //    RawItem = rawDocumentation;
        //    Description = description;
        //}

        public RDomStructuredDocumentation(RDomStructuredDocumentation oldRDom)
            :  base(oldRDom)
        { }
        public string Description { get; set; }
        public string Document { get; set; }


        //public bool SameIntent<T>(T other)
        //     where T : class
        //{
        //    var otherAnnotation = other as IStructuredDocumentation;

        //    if (RawItem != otherAnnotation.RawItem) return false;
        //    return true;
        //}

        //public bool SameIntent<T>(T otherAnnotation, bool ignoreValue)
        //    where T : class
        //{
        //    return SameIntent(otherAnnotation);
        //}

    }
}
