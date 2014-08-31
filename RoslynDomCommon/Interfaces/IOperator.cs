using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IOperator : ITypeMember<IOperator>, ICanBeStatic , IStatementContainer, IHasParameters 
    {
        // In C# it must be public and static, but other languages may not follow that rule
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
           "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Operator",
          Justification = "Because this represents an operator, it's seems an appropriate name")]
        Operator Operator { get; set; }
        IReferencedType Type { get; set; }
    }
}