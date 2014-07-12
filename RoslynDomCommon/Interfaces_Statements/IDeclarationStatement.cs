using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IDeclarationStatement : IStatement, IDom<IDeclarationStatement >
    {
        IExpression Initializer { get; }
        IReferencedType Type { get; }
        bool IsImplicitlyTyped { get; }
        bool IsConst { get; }
    }
}
