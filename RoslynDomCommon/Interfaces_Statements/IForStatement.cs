using RoslynDom.Common;

namespace RoslynDomCommon.InterfacesForStatements
{
    public interface IForStatement : ILoop
    {
        string LoopVarName { get; set; }
        IReferencedType LoopVarType { get; set; }
        bool ImplicitlyTyped { get; set; }
        int StartValue { get; set; }
        int EndValue { get; set; }
        int IncrementBy { get; set; }
    }
}
