using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IParameter : IHasAttributes, IDom<IParameter >
    {
        IReferencedType Type { get; }
        bool IsOut { get; }
        bool IsRef { get; }
        bool IsParamArray { get; }
        bool IsOptional { get; }
        int Ordinal { get; }
    }
}
