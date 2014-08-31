using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasImplementedInterfaces : IDom
    {
        RDomCollection<IReferencedType> ImplementedInterfaces { get; }
        IEnumerable<IReferencedType> AllImplementedInterfaces { get; }
    }
}
