using System;
using System.Collections.Generic;
using System.Linq;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomStructuredDocumentation : IStructuredDocumentation, IHasSameIntentMethod
    {
        private string description;

        public RDomStructuredDocumentation()
        {     }

        public RDomStructuredDocumentation(object rawDocumentation, string description)
        {
            RawItem = rawDocumentation;
            Description = description;
        }

        public RDomStructuredDocumentation(RDomStructuredDocumentation old)
        {
            RawItem = old.RawItem;
        }
        public string Description { get; set; }
      
        public object RawItem { get; set; }


        public bool SameIntent<T>(T other)
             where T : class
        {
            var otherAnnotation = other as IStructuredDocumentation;

            if (RawItem != otherAnnotation.RawItem) return false;
            return true;
        }

        public bool SameIntent<T>(T otherAnnotation, bool ignoreValue)
            where T : class
        {
            return SameIntent(otherAnnotation);
        }

    }
}
