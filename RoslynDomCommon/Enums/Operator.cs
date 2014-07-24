using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public enum Operator
    {
        Equals = 0,
        AddAssignment,
        SubtractAssignment,
        MultiplyAssignment,
        DivideAssignment,
        ModuloAssignment,
        AndAssignment,
        ExclusiveOrAssignment,
        OrAssignment,
        LeftShiftAssignment,
        RightShiftAssignment
    }
}
