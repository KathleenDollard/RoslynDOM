using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IIfStatement : IStatement
    {
        ICondition Condition { get; }
        IEnumerable<IStatement> Statements { get; }
        IEnumerable<IIfStatement> Elses { get; }
    }
}
