using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IDom : IHasSameIntentMethod
    {
        object RawItem { get; }
        object OriginalRawItem { get; }
        string Name { get; }
        string OuterName { get; }

        object RequestValue(string propertyName);

        PublicAnnotationList PublicAnnotations { get; }
    }

    public interface IDom<T> : IDom
        where T : IDom<T>
    {
        T Copy();
        bool Matches(T other);
    }
}