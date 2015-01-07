using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface INestedContainer : IDom
   {
      IEnumerable<IType> Types { get; }

      IEnumerable<IClass> Classes { get; }

      IEnumerable<IStructure> Structures { get; }

      IEnumerable<IInterface> Interfaces { get; }

      IEnumerable<IEnum> Enums { get; }
   }
}