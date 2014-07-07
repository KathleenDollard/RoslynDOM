using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IDeclarationStatement 
    {
        string VarName { get; }
        IExpression Expression { get; }
        IReferencedType Type { get; }
        bool ImplicitlyTyped { get; }
    }
}
