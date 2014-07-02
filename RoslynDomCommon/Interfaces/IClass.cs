using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IClass :
        IType<IClass>, 
        INestedContainer, 
        ITypeMemberContainer, 
        IHasTypeParameters, 
        IHasImplementedInterfaces , 
        ICanBeStatic 
    {
        bool IsAbstract { get; }
        bool IsSealed { get; }

        IReferencedType BaseType { get; }

    }
}