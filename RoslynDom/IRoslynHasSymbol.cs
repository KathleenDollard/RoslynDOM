using Microsoft.CodeAnalysis;

namespace RoslynDom
{
   public interface IRoslynHasSymbol
   {
      ISymbol Symbol { get; }
   }
}