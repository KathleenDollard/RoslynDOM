using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IEvent : IPropertyOrMethod<IEvent>
    {
        IReferencedType Type { get; set; }
        // not yet supporting event accessors
    }
}