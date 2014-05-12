using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using System.Xml.Linq;

namespace KadGen.Common
{
    public class KadMetadataClass<T> : KadMetadata<T> where T : KadMetadata<T>
    {
        public KadMetadataClass()
        {
            Interfaces = new List<string>();
        }
        public virtual string Comments { get; set; }
        public virtual string ClassName { get; set; }
        public virtual string NamespaceName { get; set; }
        public virtual ScopeAccess ScopeAccess { get; set; }
        public IEnumerable<string> Interfaces { get; private set; }
    }

    public class KadMetadataMethod<T> : KadMetadata<T> where T : KadMetadata<T>
    {
        public virtual string Name { get; set; }
        public virtual ScopeAccess ScopeAccess { get; set; }
    }

    public class KadMetadataParameter<T> : KadMetadata<T> where T : KadMetadata<T>
    {
        // TODO: Add support for Types in Roslyn parsing to allow full type names and comparisons
        public string TypeName { get; set; }
        public string Name { get; set; }
    }

    public class KadMetadata
    {
        public KadMetadata()
        {
            AdditionalAttributes = new List<KadAttribute>();
        }

        public string XmlCommentString {get; set;}

        private List<Type> _definingAttributes = new List<Type>();
        public IEnumerable<Type> DefiningAttributes
        { get { return _definingAttributes; } }

        protected Type AddDefiningAttribute(Type type)
        {
            if (typeof(Attribute).IsAssignableFrom(type))
            {
                _definingAttributes.Add(type);
                return type;
            }
            throw new InvalidOperationException("Type is not an attribute");
        }

        public List<KadAttribute> AdditionalAttributes { get; private set; }

        protected virtual IEnumerable<KadMetadata> NestedLevel
        { get { return null; } }

        protected void FixAttributes()
        {
            var type = this.GetType().GetTypeInfo();
            foreach (var attrType in this.DefiningAttributes)
            {
                var attrName = attrType.Name.Replace("Attribute", "");
                var additional = AdditionalAttributes.Where(x => x.Name == attrName).FirstOrDefault();
                if (additional == null)
                {
                    additional = new KadAttribute(attrName);
                    AdditionalAttributes.Add(additional);
                }
                var propInfos = attrType.GetTypeInfo().DeclaredProperties.Where(x => x.CanWrite);
                foreach (var propInfo in propInfos)
                {
                    var prop = additional.Properties.Where(x => x.Name == propInfo.Name).FirstOrDefault();
                    if (prop == null)
                    {
                        var thisPropInfo = type.DeclaredProperties.Where(x => x.Name == propInfo.Name).FirstOrDefault();
                        if (thisPropInfo != null)
                        {
                            var value = thisPropInfo.GetValue(this);
                            if (thisPropInfo.PropertyType == typeof(string))
                            { value = "\"" + value + "\""; }
                            additional.Properties.Add(new KadAttributeProperty(propInfo.Name, value));
                        }
                    }
                }
            }
        }

        public bool ValidateAndUpdate()
        {
            FixAttributes();
            if (!ValidateAndUpdateCore()) return false;
            if (NestedLevel != null)
            {
                foreach (var item in NestedLevel)
                { if (!item.ValidateAndUpdate()) return false; }
            }
            return true;
        }

        public virtual bool ValidateAndUpdateCore()
        { return true; }

    }

    public class KadMetadata<T> : KadMetadata where T : KadMetadata<T>
    {

    }

}
