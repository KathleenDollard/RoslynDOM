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
        bool IsAbstract { get; set; }
        bool IsSealed { get; set; }

        IReferencedType BaseType { get; set; }

    }
}