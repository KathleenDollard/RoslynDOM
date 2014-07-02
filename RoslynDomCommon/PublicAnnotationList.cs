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

        public  void AddPublicAnnotations(IEnumerable<PublicAnnotation> publicAnnotations)
        {
            if (publicAnnotations == null) return;
            foreach (var publicAnnotation in publicAnnotations )
            { this._publicAnnotations.Add(publicAnnotation); }
        }

        public void AddPublicAnnotations(PublicAnnotationList publicAnnotations)
        {
            if (publicAnnotations == null) return;
            foreach (var publicAnnotation in publicAnnotations._publicAnnotations )
            { this._publicAnnotations.Add(publicAnnotation); }
        }

        public object GetValue(string name, string key)
        {
            var annotation = GetPublicAnnotation(name);
            if (annotation == default(PublicAnnotation)) return null;
            return annotation[key];
        }

        public T GetValue<T>(string name, string key)
        {
            var annotation = GetPublicAnnotation(name);
            if (annotation == default(PublicAnnotation)) return default(T);
            return annotation.GetValue<T>(key);
        }

        public void AddValue(string name, string key, object value)
        {
            var publicAnnotation = GetPublicAnnotation(name);
            if (publicAnnotation == default(PublicAnnotation))
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
            return (GetPublicAnnotation(name) != default(PublicAnnotation));
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
        {
            foreach (var annotation in _publicAnnotations)
            {
                var otherAnnotation = otherAnnotations.GetPublicAnnotation(annotation.Name);
                if (otherAnnotation != annotation) return false;
            }
            return true;
        }
    }
}
