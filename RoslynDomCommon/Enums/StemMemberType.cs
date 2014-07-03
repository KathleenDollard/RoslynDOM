using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public enum StemMemberType
    {
        Unknown = 0,
        Using,
        Namespace,
        Class,
        Structure,
        Enum,
        Interface
    }
}
