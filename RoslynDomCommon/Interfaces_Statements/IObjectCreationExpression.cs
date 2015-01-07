using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface IObjectCreationExpression : IExpression
   {
      IReferencedType Type { get; set; }

      IEnumerable<IArgument> Arguments { get; }
   }
}