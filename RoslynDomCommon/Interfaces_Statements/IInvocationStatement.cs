using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IInvocationStatement : IStatement , IDom<IInvocationStatement>
    {
        IExpression Invocation { get; set; } // NOt ready to dive here yet
        
    }
}
