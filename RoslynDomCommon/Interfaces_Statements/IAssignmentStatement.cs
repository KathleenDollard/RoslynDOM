using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IAssignmentStatement : IStatement, IDom<IAssignmentStatement >
    {
        string VarName { get; }
        IExpression Expression { get; }
    }
}
