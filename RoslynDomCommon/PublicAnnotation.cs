using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.Common
{
    public class PublicAnnotation :  IPublicAnnotation
    {
        public PublicAnnotation (string name)
        {
            this.Name = name;
        }
        // Design note: I chose between this property bag style implementation and an attribute
        // style implementation with specific annotation classes derived from a root. I decided
        // against the class approach becuase of challenges having those classes available at 
        // design time. 

        private List<KeyValuePair<string, object>> items = new List<KeyValuePair<string, object>>();

        public string Name { get; private set;} 

        public void AddItem(string key, object item)
        { items.Add(new KeyValuePair<string, object>(key, item)); }

        public object this[string key]
        {
            get
            {
                var item = items.Where(x => x.Key == key).FirstOrDefault();
                return item.Value;
            }
        }

        public IEnumerable<string> Keys
        {
            get { return items.Select(x => x.Key); }
        }

        public bool SameIntent<T>(T other)
             where T : class
        {
            var otherAnnotation = other as IPublicAnnotation;
            foreach (var item in items)
            {
                var otherValue = otherAnnotation.GetValue(item.Key);
                if (otherValue == null) return false;
                var itemHasSameIntent = item.Value as IHasSameIntentMethod;
                if (itemHasSameIntent != null)
                { if (!itemHasSameIntent.SameIntent(otherValue)) { return false; } }
                if (!otherValue.Equals(item.Value)) return false;
            }
            return true;
        }

        public bool SameIntent<T>(T otherAnnotation, bool ignorePublicAnnotations)
            where T : class
        {
            if (ignorePublicAnnotations) return true; 
            return SameIntent(otherAnnotation);
        }


        public T GetValue<T>(string key)
        { return (T)this[key]; }

        public object GetValue(string key)
        { return this[key]; }

            public bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);
            if (!HasValue(key)) { return false; }
            value = GetValue<T>(key);
            return true;
        }

        public bool HasValue(string key)
        {
            return items.Any(x => x.Key == key);
        }

    }
}
