using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IType :  IHasAttributes, IHasNamespace, IStemMember, ITypeMember
    {
    }

    public interface IType<T> : IType, IDom,  ITypeMember<T>
        where T : IType<T>
    {
    }
}