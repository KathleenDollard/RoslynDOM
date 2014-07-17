using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasAttributes
    {
        AttributeList Attributes { get; }
    }
}
