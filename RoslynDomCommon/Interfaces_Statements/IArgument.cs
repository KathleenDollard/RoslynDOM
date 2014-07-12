using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IArgument
    {
        string Name { get; }
        bool IsRef { get; }
        bool IsOut { get; }
        IExpression ValueExpression { get; }
    }
}
