using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IReturnStatement : IStatement, IDom<IReturnStatement>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", 
            "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Return",
           Justification = "Because this represents a return value, it's a probably an appropriate name")]
        IExpression Return { get; set; }
    }
}
