using RoslynDom.Common;

namespace RoslynDomCommon.InterfacesForStatements
{
    public interface IForEachStatement : ILoop
    {
        string LoopVarName { get; set; }
        IExpression CollectionExpression { get; set; }
        bool ImplicitlyTyped { get; set; }
        IReferencedType ExplicitVarType { get; set; }
 
    }
}
