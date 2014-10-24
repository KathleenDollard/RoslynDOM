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
   public interface IRegionStart : IDetail<IRegionStart>
   {
      IRegionEnd RegionEnd { get; }
   }

   /// <summary>
   /// 
   /// </summary>
   public interface IRegionEnd : IDetail<IRegionEnd>
   {
      IRegionStart RegionStart { get; }
   }
}
