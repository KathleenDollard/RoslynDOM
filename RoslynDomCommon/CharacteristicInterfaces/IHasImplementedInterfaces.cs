using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasImplementedInterfaces : IDom
    {
        RDomList<IReferencedType> ImplementedInterfaces { get; }
        IEnumerable<IReferencedType> AllImplementedInterfaces { get; }
    }
}
