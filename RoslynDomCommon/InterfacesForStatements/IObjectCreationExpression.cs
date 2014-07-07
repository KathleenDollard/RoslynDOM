using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IObjectCreationExpression : IExpression 
    {
        IReferencedType Type{ get; } 
        IEnumerable<IArgument > Arguments { get; }
    }
}
