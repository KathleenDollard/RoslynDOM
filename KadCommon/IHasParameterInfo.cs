using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KadGen.Common
{
    public interface IHasParameterInfo
    {
        string TypeName { get; set; }
        string Name { get; set; }
    }
}
