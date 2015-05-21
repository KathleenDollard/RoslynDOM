using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IStructuredDocumentationElement 
      :  IHasSameIntentMethod , IMisc, IDom<IStructuredDocumentationElement>, IHasName , IHasAttributes 
    {
      string Text { get; set; }

    }
}