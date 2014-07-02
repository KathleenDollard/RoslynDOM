using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IType :  IHasAttributes, IStemMember
    {
    }

    public interface IType<T> : IType, IDom<T>,  ITypeMember<T>
        where T : IType<T>
    {
    }
}