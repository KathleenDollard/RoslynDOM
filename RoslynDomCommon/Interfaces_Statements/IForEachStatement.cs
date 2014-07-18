using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IForEachStatement : ILoop<IForEachStatement>
    {
        IVariableDeclaration Variable { get; set; }
    }
}
