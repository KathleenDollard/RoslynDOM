using System.Collections.Generic;

namespace RoslynDom.Common
{
   public interface IReferencedType : IMisc, IDom<IReferencedType>, IHasNamespace, IHasName
   {
      bool DisplayAlias { get; set; }
      bool IsArray { get; set; }
      RDomCollection<IReferencedType> TypeArguments { get; }
   }
}