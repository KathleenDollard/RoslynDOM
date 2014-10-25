using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   public interface ITriviaFactory : IWorker
   {
        IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IPublicAnnotation publicAnnotation);
   }
   public interface ITriviaFactory<T> : ITriviaFactory
   {
      /// <summary>
      /// Returns the public annotation, or null if there is no match
      /// </summary>
      /// <param name="comment"></param>
      /// <returns></returns>
      T CreateFrom(string possibleAnnotation, RDomCorporation corporation);

      /// <summary>
      /// Returns the public annotation, or null if there is no match
      /// </summary>
      /// <param name="comment"></param>
      /// <returns></returns>
      T CreateFrom(SyntaxTrivia trivia, RDomCorporation corporation);
   }
}
