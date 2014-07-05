using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class PublicAnnotationList
    {
        private List<PublicAnnotation> _publicAnnotations = new List<PublicAnnotation>();

        public void Add(IEnumerable<PublicAnnotation> publicAnnotations)
        {
            if (publicAnnotations == null) return;
            foreach (var publicAnnotation in publicAnnotations)
            { this._publicAnnotations.Add(publicAnnotation); }
        }

        public void AddCopy(PublicAnnotationList publicAnnotations)
        {
            if (publicAnnotations == null) return;
            foreach (var publicAnnotation in publicAnnotations._publicAnnotations)
            {
                foreach (var key in publicAnnotation.Keys)
                { this.AddValue(publicAnnotation.Name, key, publicAnnotation.GetValue(key)); }
            }
        }

        public object GetValue(string name, string key)
        {
            var annotation = GetPublicAnnotation(name);
            if (annotation == null) return null;
            return annotation.GetValue(key);
        }

        public T GetValue<T>(string name, string key)
        {
            var annotation = GetPublicAnnotation(name);
            if (annotation == null) return default(T);
            return annotation.GetValue<T>(key);
        }

        public void AddValue(string name, string key, object value)
        {
            var publicAnnotation = GetPublicAnnotation(name);
            if (publicAnnotation == null)
            {
                publicAnnotation = new PublicAnnotation(name);
                _publicAnnotations.Add(publicAnnotation);
            }
            publicAnnotation.AddItem(key, value);
        }

        public void AddValue(string name, object value)
        {
            AddValue(name, name, value);
        }

        private PublicAnnotation GetPublicAnnotation(string name)
        {
            return _publicAnnotations
                                .Where(x => x.Name == name)
                                .FirstOrDefault();
        }

        public bool HasPublicAnnotation(string name)
        {
            return (GetPublicAnnotation(name) != null);
        }

        public object GetValue(string name)
        {
            return GetValue(name, name);
        }

        public T GetValue<T>(string name)
        {
            return GetValue<T>(name, name);
        }

        public bool SameIntent(PublicAnnotationList otherAnnotations)
        { return SameIntent(otherAnnotations, true); }

        public bool SameIntent(PublicAnnotationList otherAnnotations, bool includePublicAnnotations)
        {
            if (!includePublicAnnotations) return true;
            foreach (var annotation in _publicAnnotations)
            {
                var otherAnnotation = otherAnnotations.GetPublicAnnotation(annotation.Name);
                if (!annotation.SameIntent(otherAnnotation)) return false;
            }
            return true;
        }
    }
}
