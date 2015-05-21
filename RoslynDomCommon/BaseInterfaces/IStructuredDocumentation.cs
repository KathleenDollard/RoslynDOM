using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IStructuredDocumentation :  IHasSameIntentMethod , IMisc, IDom<IStructuredDocumentation>
    {
      RDomCollection<IStructuredDocumentationElement> Elements { get;}

    }
}