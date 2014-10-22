using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   /// <summary>
   /// 
   /// </summary>
   public interface IRegionStart : IDirective, IDom<IRegionStart>
   {
      IRegionEnd RegionEnd { get; }
   }

   /// <summary>
   /// 
   /// </summary>
   public interface IRegionEnd : IDirective, IDom<IRegionEnd>
   {
      IRegionStart RegionStart { get; }
   }
}
