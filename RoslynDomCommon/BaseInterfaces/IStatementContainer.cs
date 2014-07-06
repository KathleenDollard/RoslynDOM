using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IStatementContainer 
    {
        IEnumerable<IStatement> Statements { get; }
    }
}
