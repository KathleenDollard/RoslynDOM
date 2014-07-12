using System.Collections.Generic;
using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface ITryStatement : IStatement
    {
        IEnumerable<IStatement> Statements { get; }
        IEnumerable<ICatch> Catches { get; }
        IEnumerable<IStatement > Finally { get; }
    }
}
