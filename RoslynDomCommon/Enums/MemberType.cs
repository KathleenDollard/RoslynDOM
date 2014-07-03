using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public enum MemberType
    {
        Unknown = 0,
        Class,
        Structure,
        Enum,
        Interface,
        Method,
        Property,
        Field,
        EnumItem,
        InvalidMember
    }
}
