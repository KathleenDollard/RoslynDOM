using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class PublicAnnotation(string name)
    {
        // Design note: I chose between this property bag style implementation and an attribute
        // style implementation with specific annotation classes derived from a root. I decided
        // against the class approach becuase of challenges having those classes available at 
        // design time. 

        private List<KeyValuePair<string, object>> items = new List<KeyValuePair<string, object>>();

        public string Name { get; } = name;

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

        public T GetValue<T>(string key)
        { return (T)this[key]; }

        public object GetValue(string key)
        { return this[key]; }

    }
}
