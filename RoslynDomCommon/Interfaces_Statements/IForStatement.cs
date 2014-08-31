using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IForStatement : ILoop<IForStatement>
    {
        IVariableDeclaration Variable { get; set; }
 
        IExpression  Iterator { get; set; }
    }
}
