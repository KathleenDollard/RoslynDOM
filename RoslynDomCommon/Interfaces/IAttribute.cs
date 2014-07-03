using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IAttribute : IDom<IAttribute>
    {
        // TODO: Determine how AllowMultiple will play
        // Not including Attribute usage, scope and other info about the attribute class at this point
        // Do not currently see a use for an IHasProperties interface for Class/Attribute similarities
        // Not supporting methods on attributes
        IEnumerable<IAttributeValue> AttributeValues { get; }
    }
}
