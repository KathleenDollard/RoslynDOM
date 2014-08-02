using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IArgument : IDom<IArgument >, IHasName
    {
        bool IsRef { get; set; }
        bool IsOut { get; set; }
        IExpression ValueExpression { get; set; }
    }
}
