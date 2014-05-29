using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IHasImplementedInterfaces
    {
        IEnumerable<IReferencedType> ImplementedInterfaces { get; }
        IEnumerable<IReferencedType> AllImplementedInterfaces { get; }
    }
}
