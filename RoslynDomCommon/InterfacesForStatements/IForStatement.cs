using RoslynDom.Common;

namespace RoslynDomCommon.InterfacesForStatements
{
    public interface IForStatement : ILoop
    {
        string LoopVarName { get; }
        IReferencedType LoopVarType { get; }
        bool ImplicitlyTyped { get; }
        int StartValue { get; }
        int EndValue { get; }
        int IncrementBy { get; }
    }
}
