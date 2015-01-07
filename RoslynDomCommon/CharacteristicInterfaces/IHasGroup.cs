using System;

namespace RoslynDom.Common
{
   public interface IHasGroup : IDom
   {
      Guid GroupGuid { get; }
   }
}