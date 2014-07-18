using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface ILoop<T> :IStatement, IDom<T>
        where T : IDom<T>
    {
        void RemoveStatement(IStatement statement);
        void AddOrMoveStatement(IStatement statement);
        IEnumerable<IStatement> Statements { get; }
        bool TestAtEnd { get; set; }
        bool HasBlock { get; set; }

        IExpression Condition { get; set; }
    }
}
