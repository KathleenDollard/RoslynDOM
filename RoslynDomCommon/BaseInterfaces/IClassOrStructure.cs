using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IClassOrStructure : ITypeMemberContainer
    {
        IEnumerable<IField> Fields { get; }
        IEnumerable<IConversionOperator> ConversionOperators { get; }
        IEnumerable<IOperator> Operators { get; }
        IEnumerable<IConstructor> Constructors { get; }
    }
}
