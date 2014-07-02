using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IPropertyOrMethod : IHasReturnType, ICanBeStatic
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
