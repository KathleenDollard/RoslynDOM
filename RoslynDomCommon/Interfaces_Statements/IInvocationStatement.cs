using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IInvocationStatement : IStatement , IDom<IInvocationStatement>
    {
        IExpression Invocation { get; set; } // Not ready to dive here yet
        
    }
}
