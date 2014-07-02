using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   public interface IAttributeValue : IDom<IAttributeValue >
    {
        LiteralType  ValueType { get; }
        object Value { get; }
    }
}
