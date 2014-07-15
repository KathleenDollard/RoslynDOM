using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IDeclarationStatement : IStatement, IDom<IDeclarationStatement >
    {
        IExpression Initializer { get; set; }
        IReferencedType Type { get; set; }
        bool IsImplicitlyTyped { get; set; }
        bool IsConst { get; set; }
    }
}
