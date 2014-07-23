using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IAssignmentStatement : IStatement, IDom<IAssignmentStatement >
    {
        IExpression Expression { get; set; }
        IExpression Left { get; set; }

    }
}
