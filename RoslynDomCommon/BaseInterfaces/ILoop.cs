using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface ILoop<T> :IStatement, IDom<T>, IHasCondition, IStatementContainer
        where T : IDom<T>
    {
      
        bool TestAtEnd { get; set; }

    }
}
