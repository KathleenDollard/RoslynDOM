using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasImplementedInterfaces : IDom
    {
        IEnumerable<IReferencedType> ImplementedInterfaces { get; }
        IEnumerable<IReferencedType> AllImplementedInterfaces { get; }
    }
}
