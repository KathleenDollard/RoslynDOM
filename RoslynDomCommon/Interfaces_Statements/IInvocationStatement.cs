using RoslynDom.Common;
using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface IInvocationStatement : IHasInvocationFeatures, IStatement, IDom<IInvocationStatement>
   {
      IInvocationExpression Invocation { get; set; }
   }
}
