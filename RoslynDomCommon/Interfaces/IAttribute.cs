using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IAttribute : IDom<IAttribute>, IMisc
    {
        // TODO: Consider: Determine how AllowMultiple will play
        // TODO: Consider: Not supporting methods on attributes yet
        // TODO: Consider: Not including Attribute usage, scope and other info about the attribute class at this point
        // Do not currently see a use for an IHasProperties interface for Class/Attribute similarities
        IEnumerable<IAttributeValue> AttributeValues { get; }
    }
}
