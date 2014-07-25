using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.Common
{
    public class PublicAnnotationList : IHasLookupValue
    {
        private List<IPublicAnnotation> _publicAnnotations = new List<IPublicAnnotation>();

        public void Add(IEnumerable<IPublicAnnotation> publicAnnotations)
        {
            if (publicAnnotations == null) return;
            foreach (var publicAnnotation in publicAnnotations)
            { this._publicAnnotations.Add(publicAnnotation); }
        }

        public IEnumerable<PublicAnnotation> Copy()
        {
            var ret = new List<PublicAnnotation>();

            foreach (var publicAnnotation in this._publicAnnotations)
            {
                var newAnnotation = new PublicAnnotation(publicAnnotation.Name);
                foreach (var key in publicAnnotation.Keys)
                {
                    newAnnotation.AddItem(key, publicAnnotation.GetValue(key));
                }
                ret.Add(newAnnotation);
            }
            return ret;
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

        public IPublicAnnotation GetPublicAnnotation(string name)
        {
            return _publicAnnotations
                                .Where(x => x.Name == name)
                                .FirstOrDefault();
        }

        public bool TryGetPublicAnnotation(string name, out IPublicAnnotation publicAnnotation)
        {
            publicAnnotation = null;
            if (!HasPublicAnnotation(name)) { return false; }
            publicAnnotation = GetPublicAnnotation(name);
            return true;
        }

        public bool HasPublicAnnotation(string name)
        {
            foreach (var publicAnnotation in _publicAnnotations)
            {
                if (publicAnnotation.Name == name) { return true; }
            }
            return false;
        }


        public bool SameIntent(PublicAnnotationList otherAnnotations)
        { return SameIntent(otherAnnotations, false); }

        public bool SameIntent(PublicAnnotationList otherAnnotations, bool skipPublicAnnotations)
        {
            if (skipPublicAnnotations) return true;
            if (this._publicAnnotations.Count != otherAnnotations._publicAnnotations.Count) return false;
            foreach (var annotation in _publicAnnotations)
            {
                var otherAnnotation = otherAnnotations.GetPublicAnnotation(annotation.Name);
                if (!annotation.SameIntent(otherAnnotation)) return false;
            }
            return true;
        }

        public bool HasValue(string name, string key)
        {
            foreach (var publicAnnotation in _publicAnnotations)
            {
                if (publicAnnotation.Name == name)
                { return publicAnnotation.HasValue(key); }
            }
            return false;
        }

        public bool TryGetValue(string name, string key, out object value)
        {
            value = null;
            if (!HasValue(name, key)) { return false; }
            value = GetValue(name, key);
            return true;
        }

        public bool TryGetValue<T>(string name, string key, out T value)
        {
            value = default(T);
            if (!HasValue(name, key)) { return false; }
            value = GetValue<T>(name, key);
            return true;
        }

        object IHasLookupValue.GetValue(string key)
        {
            var publicAnnotation = GetPublicAnnotation(key);
            return publicAnnotation;
        }

        T IHasLookupValue.GetValue<T>(string key)
        {
            if (!typeof(T).IsAssignableFrom(typeof(PublicAnnotation))) { throw new InvalidOperationException(); }
            return (T)(GetPublicAnnotation(key) as object);
        }

        bool IHasLookupValue.HasValue(string key)
        {
            return HasPublicAnnotation(key);
        }

           bool IHasLookupValue.TryGetValue<T>(string key, out T value)
        {
            if (!typeof(T).IsAssignableFrom(typeof(PublicAnnotation))) { throw new InvalidOperationException(); }
            IPublicAnnotation annot;
            var ret = TryGetPublicAnnotation(key, out annot);
            if (!ret) { value = default(T); return false; }
            value = (T)(annot as object);
            return true;
        }
    }
}
