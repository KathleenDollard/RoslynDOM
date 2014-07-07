using RoslynDom.Common;

namespace RoslynDomCommon.InterfacesForStatements
{
    public interface IForEachStatement : ILoop
    {
        string LoopVarName { get; }
        IExpression CollectionExpression { get; }
        bool ImplicitlyTyped { get; }
        IReferencedType ExplicitVarType { get; }
 
    }
}
