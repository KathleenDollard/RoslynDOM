using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.CSharp
{
   public interface IPublicAnnotationMatcher
   {
      /// <summary>
      /// Returns the public annotation, or null if there is no match
      /// </summary>
      /// <param name="comment"></param>
      /// <returns></returns>
      IPublicAnnotation GetFromComment(string possibleAnnotation, RDomCorporation corporation);
   }
}
