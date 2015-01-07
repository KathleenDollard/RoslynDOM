using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface IPublicAnnotation : IDetail<IPublicAnnotation>, IHasLookupValue, IHasSameIntentMethod
   {
      IEnumerable<string> Keys { get; }

      string Name { get; set; }

      string Target { get; set; }

      void AddItem(string key, object item);
   }
}