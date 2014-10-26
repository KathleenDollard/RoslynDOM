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
   public interface IDetailBlockStart : IDetail<IDetailBlockStart>
   {
      IDetailBlockEnd BlockEnd { get; }
      string Text { get; set; }
      string BlockStyleName { get; }
   }
}
