using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IAssignmentStatement 
    {
        string VarName { get; }
        IExpression Expression { get; }
    }
}
