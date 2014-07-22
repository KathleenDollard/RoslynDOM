using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IPropertyOrMethod : IHasReturnType, ICanBeStatic, ITypeMember
    {
        bool IsAbstract { get; set; }
        bool IsVirtual { get; set; }
        bool IsOverride { get; set; }
        bool IsSealed { get; set; }

        RDomList<IParameter> Parameters { get; }
    }

    public interface IPropertyOrMethod<T> : IPropertyOrMethod, ITypeMember<T>
        where T : IPropertyOrMethod<T>
    { }
}
