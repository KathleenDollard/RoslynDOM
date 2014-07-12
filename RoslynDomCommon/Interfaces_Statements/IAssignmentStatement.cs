using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IAssignmentStatement : IStatement
    {
        string VarName { get; }
        IExpression Expression { get; }
    }
}
