using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public enum LanguagePart
    {
        Block,
        Current,
        Inner,
        Variable,
        Condition,
        Incrementor,
        Type,
        AccessorList,
        AttributeList,
        AttributeArgumentList,
        AttributeName,
        Expression
    }
}
