using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public struct PublicAnnotation(string name)
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

        #region Equality
        // This is a bit hacked together and could probably use review

        public override bool Equals(object obj)
        {
            if (obj is PublicAnnotation)
            {
                var p = (PublicAnnotation)obj;
                return this.Equals(p);
            }
            return false;
        }

        public bool Equals(PublicAnnotation other)
        {
            // Return true if the names and values match:
            if (this.Name != other.Name) return false;
            if (this.items == null )return (other.items == null);
            foreach (var itemX in this.items)
            {
                var hasItem = other.items
                                .Where(z => z.Key == itemX.Key && z.Value == itemX.Value)
                                .Any();
                if (!hasItem) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            var str = this.Name + "+_+";
            // Don't care about order, but need to sort so two equal annotations have same hash code
            foreach (var itemX in this.items.OrderBy(x=>x.Key).ThenBy(x=>x.Value))
            {
                str += itemX.Key + "%$" + itemX.Value + "*|";
            }
            return str.GetHashCode();
        }

        public static bool operator ==(PublicAnnotation  left, PublicAnnotation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PublicAnnotation left, PublicAnnotation right)
        {
            return !(left == right);
        }
        #endregion

    }
}
