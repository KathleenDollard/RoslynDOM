using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public enum Operator
    {
        Unknown = 0,
        Plus,
        Minus,
        Asterisk,
        Slash,
        Percent,
        Ampersand,
        Bar,
        Caret,
        LessThanLessThan,
        GreaterThanGreaterThan,
        EqualsEquals,
        ExclamationEquals,
        GreaterThan,
        LessThan,
        GreaterThanEquals,
        LessThanEquals
    }
}