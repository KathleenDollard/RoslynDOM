using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IPropertyOrMethod : IHasReturnType, ICanBeStatic, ITypeMember
    {
        bool IsAbstract { get; }
        bool IsVirtual { get; }
        bool IsOverride { get; }
        bool IsSealed { get; }

        IEnumerable<IParameter> Parameters { get; }
    }

    public interface IPropertyOrMethod<T> : IPropertyOrMethod, ITypeMember<T>
        where T : IPropertyOrMethod<T>
    { }
}
