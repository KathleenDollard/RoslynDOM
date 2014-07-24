using System.Collections.Generic;
using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IUsingStatement : IStatement, IStatementBlock  , IDom<IUsingStatement >
    {
       IExpression Expression { get; set; }
        IVariableDeclaration Variable { get; set; }
    }
}
