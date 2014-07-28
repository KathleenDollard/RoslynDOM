using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IStructuredDocumentation :  IHasSameIntentMethod , IMisc, IDom<IStructuredDocumentation>
    {
        string Description { get; set; }
        string Document { get; set; }

    }
}