using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface ICatch : IStatement, IHasCondition, IStatementBlock
    {
        string ExceptionName { get; set; }
        string ExceptionType { get; set; } // For .NET, this should be IReferencedType, not sure on other platforms
    }
}
