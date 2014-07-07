using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IReturnStatement : IStatement
    {
        IExpression Return { get; }
    }
}
