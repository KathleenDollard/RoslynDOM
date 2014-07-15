using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IArgument
    {
        string Name { get; set; }
        bool IsRef { get; set; }
        bool IsOut { get; set; }
        IExpression ValueExpression { get; set; }
    }
}
