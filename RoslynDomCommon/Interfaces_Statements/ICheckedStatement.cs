using System.Collections.Generic;
using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface ICheckedStatement : IStatement, IStatementBlock, IDom<ICheckedStatement>
    {
        bool Unchecked { get; set; } // perhaps this should be a differnet statement
    }
}
