using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDomUtilities
{
    public static class ListUtilities
    { 
        public static IEnumerable<T> MakeList<T, TInput, TRaw>(
              TInput input,
              Func<TInput, IEnumerable<TRaw>> getItemsDeleg,
              Func<TRaw, T> makeNewItem)
        {
            var ret = new List<T>();
            foreach (var rawItem in getItemsDeleg(input))
            {
                var item = makeNewItem(rawItem);
                ret.Add(item);
            }
            return ret;
        }
    }
}
