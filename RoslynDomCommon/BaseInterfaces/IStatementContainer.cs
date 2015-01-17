using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IStatementContainer : 
      IDom, IStatementBlock, 
      IHasName, IContainer
    {   
    }
}
