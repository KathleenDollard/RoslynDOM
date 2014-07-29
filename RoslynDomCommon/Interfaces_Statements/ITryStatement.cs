using System.Collections.Generic;
using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface ITryStatement : IStatement, IDom<ITryStatement>, IStatementBlock
    {
        IEnumerable<ICatchStatement> Catches { get; }
        IFinallyStatement Finally { get; }
    }

    public interface ICatchStatement : IStatement, IHasCondition, IStatementBlock, IDom<ICatchStatement >
    {
        IVariableDeclaration Variable { get; set; }
        IReferencedType ExceptionType { get; set; }

        //TODO: Add conditionals for C# 6
    }

    public interface IFinallyStatement : IStatement, IDom<IFinallyStatement>, IStatementBlock
    {
    }
}


