using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KadGen.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class KadAttribute : Attribute
    {
        public KadAttribute(string name)
        { 
            this.Name = name;
            Properties = new List<KadAttributeProperty>(); 
        }

        public string Name { get; private set; }

        public List<KadAttributeProperty> Properties {get; private set;}
    }

    public class KadAttributeProperty
    {
        public KadAttributeProperty(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        public KadAttributeProperty(int ordinal, object value)
        {
            this.Ordinal = ordinal;
            this.Value = value;
        }

        public string Name { get; private set; }
        public int Ordinal { get; private set; }
        public object Value { get; private set; }
    }
}
