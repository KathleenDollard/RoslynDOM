using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IPublicAnnotation :IHasLookupValue  , IHasSameIntentMethod , IMisc, IDom<IPublicAnnotation>
    {
        object this[string key] { get; }

        IEnumerable<string> Keys { get; }
        string Name { get; }

        void AddItem(string key, object item);
      
    }
}