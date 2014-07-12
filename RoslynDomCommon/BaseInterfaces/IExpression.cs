using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IExpression
    {
        string Expression { get; } // at present this would be a rathole, particularly between languages
    }
}
