using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IDeclarationStatement : IStatement, IDom<IDeclarationStatement >, IVariable
    {
        bool IsConst { get; set; }

    }
}
