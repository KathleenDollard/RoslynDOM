using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IDom
    {
        object RawItem { get; }
        string Name { get; }
        string OuterName { get; }

        object RequestValue(string propertyName);

        PublicAnnotationList PublicAnnotations { get; }
    }

    public interface IDom<T> : IDom
        where T : IDom<T>
    {
        T Copy();
        bool SameIntent(T other, bool includePublicAnnotations);
        bool SameIntent(T other);
    }
}