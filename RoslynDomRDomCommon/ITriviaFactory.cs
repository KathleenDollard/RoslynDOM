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
