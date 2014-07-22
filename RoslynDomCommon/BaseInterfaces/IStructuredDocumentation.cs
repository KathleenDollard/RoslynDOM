using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IStructuredDocumentation :  IHasSameIntentMethod 
    {
        object RawItem { get; set; }
        string Description { get; set; }
      
    }
}