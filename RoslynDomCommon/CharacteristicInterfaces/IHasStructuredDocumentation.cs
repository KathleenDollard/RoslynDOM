using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasStructuredDocumentation : IDom
    {
        IStructuredDocumentation StructuredDocumentation { get; set; }
        string Description { get; set;  }
    }
}
