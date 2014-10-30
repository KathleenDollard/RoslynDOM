using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.Common
{
   public static class IDomUtilities
   {
      public static IEnumerable<T> NoWhitespace<T>(this IEnumerable<T> list)
         where T : IDom
      {
         return list.Where(x=>!(x is IVerticalWhitespace));
      }
   }
}
