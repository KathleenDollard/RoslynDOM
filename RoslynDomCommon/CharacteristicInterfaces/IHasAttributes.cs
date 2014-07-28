using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasAttributes : IDom
    {
        AttributeList Attributes { get; }
    }
}
