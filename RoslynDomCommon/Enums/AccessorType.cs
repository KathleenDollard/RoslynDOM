using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public enum AccessorType
    {
        Unknown = 0,
        Get,
        Set,
        AddEventAccessor,
        RemoveEventAccessor
    }
}
