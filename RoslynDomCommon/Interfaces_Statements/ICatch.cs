using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface ICatch : IStatement
    {
        IExpression Condition { get; }
        string ExceptionName { get; }
        string ExceptionType { get; } // For .NET, this should be IReferencedType, not sure on other platforms
        IEnumerable<IStatement> Statements { get; }
    }
}
