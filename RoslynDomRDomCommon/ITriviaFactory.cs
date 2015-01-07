using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface ITriviaFactory : ICorporationWorker
   {
      /// <summary>
      /// Returns the detail, or null if there is no match
      /// </summary>
      /// <param name="comment"></param>
      /// <returns></returns>
      IDom CreateFrom(SyntaxTrivia trivia, IDom parent, OutputContext context);

      IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IDom item, OutputContext context);
   }

   public interface ITriviaFactory<T> : ITriviaFactory
   {
   }
}