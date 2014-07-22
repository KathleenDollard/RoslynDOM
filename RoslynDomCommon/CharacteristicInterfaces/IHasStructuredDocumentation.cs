using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasStructuredDocumentation
    {
        IStructuredDocumentation StructuredDocumentation { get; set; }
        string Description { get; set;  }
    }
}
