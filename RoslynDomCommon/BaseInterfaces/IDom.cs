using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IDom
    {
        object RawItem { get; }
        string Name { get; }
        //string QualifiedName { get; }
        string OuterName { get; }
        //string Namespace { get; }

        object RequestValue(string name);

        bool HasPublicAnnotation(string name);

        void AddPublicAnnotationValue(string name, string key, object value);
        void AddPublicAnnotationValue(string name, object value);

        object GetPublicAnnotationValue(string name, string key);
        object GetPublicAnnotationValue(string name);
        T GetPublicAnnotationValue<T>(string name);
        T GetPublicAnnotationValue<T>(string name, string key);
    }
}