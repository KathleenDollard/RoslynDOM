using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public enum LanguagePart
    {
        None = 0,
        Block,
        Current,
        Inner,
        Variable,
        Condition,
        Iterator,
        Type,
        AccessorList,
        AttributeList,
        AttributeArgumentList,
        AttributeName,
        Expression,
        Constraint,
        Initializer
    }
}
