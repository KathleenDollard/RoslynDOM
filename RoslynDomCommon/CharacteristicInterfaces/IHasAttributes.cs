using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasAttributes
    {
        IEnumerable<IAttribute> Attributes { get; }
    }
}
