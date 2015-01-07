using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface IClassOrStructure : ITypeMemberContainer
   {
      IEnumerable<IField> Fields { get; }

      IEnumerable<IConversionOperator> ConversionOperators { get; }

      IEnumerable<IOperator> Operators { get; }

      IEnumerable<IConstructor> Constructors { get; }
   }
}