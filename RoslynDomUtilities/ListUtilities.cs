using System;
using System.Collections.Generic;

namespace RoslynDom.Common
{
    public static class ListUtilities
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IEnumerable<T> MakeList<T, TInput, TRaw>(
              TInput input,
              Func<TInput, IEnumerable<TRaw>> getItemsDelegate,
              Func<TRaw, IEnumerable<T>> makeNewItems)
        {
            var ret = new List<T>();
            if (input == null) return ret;
            if (getItemsDelegate == null) throw new InvalidOperationException();
            if (makeNewItems == null) throw new InvalidOperationException();
            foreach (var rawItem in getItemsDelegate(input))
            {
                var items = makeNewItems(rawItem);
                ret.AddRange(items);
            }
            return ret;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IEnumerable<T> MakeList<T, TInput, TRaw>(
               TInput input,
               Func<TInput, IEnumerable<TRaw>> getItemsDelegate,
               Func<TRaw, T> makeNewItem)
        {
            var ret = new List<T>();
            if (input == null) return ret;
            if (input == null) return ret;
            if (getItemsDelegate == null) throw new InvalidOperationException();
            if (makeNewItem == null) throw new InvalidOperationException();
            foreach (var rawItem in getItemsDelegate(input))
            {
                var item = makeNewItem(rawItem);
                ret.Add(item);
            }
            return ret;
        }
    }
}
