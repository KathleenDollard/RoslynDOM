using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IPropertyOrMethod : 
        IHasReturnType, 
        ICanBeStatic, 
        ITypeMember, 
        IOOTypeMember, 
        ICanBeNew, 
        IHasParameters 
    {
    }

    public interface IPropertyOrMethod<T> : IPropertyOrMethod, ITypeMember<T>
        where T : IPropertyOrMethod<T>
    { }
}
