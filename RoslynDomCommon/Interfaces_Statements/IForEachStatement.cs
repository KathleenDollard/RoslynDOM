using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IForEachStatement : ILoop<IForEachStatement>
    {
        string LoopVarName { get; set; }
        IExpression CollectionExpression { get; set; }
        bool ImplicitlyTyped { get; set; }
        IReferencedType ExplicitVarType { get; set; }
 
    }
}
