using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IType :  IDom, IHasAttributes, IHasNamespace, IStemMember, ITypeMember, ICanBeNested, IHasStructuredDocumentation 
    {
    }

    public interface IType<T> : IType,   ITypeMember<T>
        where T : IType<T>
    {
    }
}