using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IOperator : ITypeMember<IOperator>, ICanBeStatic , IStatementContainer, IHasParameters 
    {
        // In C# it must be public and static, but other languages may not follow that rule
        Operator Operator { get; set; }
        IReferencedType Type { get; set; }
    }
}