using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RoslynDom.Common
{
    public class PublicAnnotation : RDomBase, IPublicAnnotation
    {
        public PublicAnnotation(string name)
        {
            this.Name = name;
        }
 
        private List<KeyValuePair<string, object>> items = new List<KeyValuePair<string, object>>();

        public string Name { get; private set; }

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

        [ExcludeFromCodeCoverage]
        public override object RequestValue(string propertyName)
        { return GetValue(propertyName); }

        protected override bool SameIntentInternal<TLocal>(TLocal other, bool skipPublicAnnotations)
        {
            if (skipPublicAnnotations) return true;
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

        [ExcludeFromCodeCoverage]
        public IPublicAnnotation Copy()
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public override object OriginalRawItem
        { get { return null; } }

        [ExcludeFromCodeCoverage]
        public override object RawItem
        { get { return null; } }

    }
}
