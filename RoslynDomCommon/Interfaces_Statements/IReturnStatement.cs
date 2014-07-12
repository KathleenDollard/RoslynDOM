using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IReturnStatement : IStatement, IDom<IReturnStatement>
    {
        IExpression Return { get; }
    }
}
