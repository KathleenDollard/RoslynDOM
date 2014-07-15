using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IIfStatement : IStatement, IDom<IIfStatement >
    {
        IExpression  Condition { get; }
        IEnumerable<IStatement> Statements { get; }
        IEnumerable<IIfStatement> ElseIfs { get; }
        IEnumerable<IStatement> ElseStatements { get; }
        bool HasBlock { get; }
        bool ElseHasBlock { get; }
    }
}
