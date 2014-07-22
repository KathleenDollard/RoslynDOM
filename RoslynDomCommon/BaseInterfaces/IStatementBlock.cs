using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IStatementBlock : IDom
    {
        void AddOrMoveStatement(IStatement member);
        void RemoveStatement(IStatement member);
        IEnumerable<IStatement> Statements { get; }
        bool HasBlock { get; set; }
    }
}
