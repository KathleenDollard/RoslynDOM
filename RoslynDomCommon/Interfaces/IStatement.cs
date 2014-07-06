using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IStatement : IDom<IStatement>, IStatementContainer
    {
        bool IsBlock { get; }
        StatementKind StatementKind { get; }
    }
}
