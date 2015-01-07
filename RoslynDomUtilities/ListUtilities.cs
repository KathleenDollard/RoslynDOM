namespace RoslynDom.Common
{
   public static class ListUtilities
   {
      //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
      //public static IEnumerable<T> MakeList<T, TInput, TRaw>(
      //      TInput input,
      //      Func<TInput, IEnumerable<TRaw>> getItemsDelegate,
      //      Func<TRaw, IEnumerable<T>> makeNewItems)
      //{
      //    var ret = new List<T>();
      //    if (input == null) return ret;
      //    if (getItemsDelegate == null) throw new InvalidOperationException();
      //    var rawItems = getItemsDelegate(input);
      //    return CreateFromList(rawItems, makeNewItems);
      //}

      //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
      //    "CA1006:DoNotNestGenericTypesInMemberSignatures",
      //    Justification = "Apparently the problem is the Func returning a generic list. This is a good usage")]
      //public static IEnumerable<T> CreateFromList<T, TRaw>(
      //       IEnumerable<TRaw> rawItems,
      //       Func<TRaw, IEnumerable<T>> makeNewItems)
      //{
      //   if (rawItems == null) throw new InvalidOperationException("rawItems can't be null");
      //    if (makeNewItems == null) throw new InvalidOperationException("makeNewItems can't be null");
      //    var ret = new List<T>();
      //    foreach (var rawItem in rawItems)
      //    {
      //        var items = makeNewItems(rawItem);
      //        if (items != null)
      //        { ret.AddRange(items.Where(x => x != null)); }
      //    }
      //    return ret;
      //}

      //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
      //public static IEnumerable<T> MakeList<T, TInput, TRaw>(
      //       TInput input,
      //       Func<TInput, IEnumerable<TRaw>> getItemsDelegate,
      //       Func<TRaw, T> makeNewItem)
      //{
      //    var ret = new List<T>();
      //    if (input == null) return ret;
      //    if (getItemsDelegate == null) throw new InvalidOperationException();
      //    if (makeNewItem == null) throw new InvalidOperationException();
      //    var newItems = getItemsDelegate(input);
      //    foreach (var rawItem in newItems)
      //    {
      //        var item = makeNewItem(rawItem);
      //        if (item != null) { ret.Add(item); }
      //    }
      //    return ret;
      //}
   }
}