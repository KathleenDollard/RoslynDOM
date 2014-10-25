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
      ///// <summary>
      ///// Returns the public annotation, or null if there is no match
      ///// </summary>
      ///// <param name="comment"></param>
      ///// <returns></returns>
      //IDom CreateFrom(string possibleAnnotation, OutputContext context);

      /// <summary>
      /// Returns the detail, or null if there is no match
      /// </summary>
      /// <param name="comment"></param>
      /// <returns></returns>
      IDom CreateFrom(SyntaxTrivia trivia, OutputContext context);

      IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IDom item, OutputContext context);
   }
   public interface ITriviaFactory<T> : ITriviaFactory
   {
 
   }
}
