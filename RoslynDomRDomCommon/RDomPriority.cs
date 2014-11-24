using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom
{
   /// <summary>
   /// Priority for candidate selection. These are for clarity. Please add your
   /// own in the format "Normal + 1"
   /// </summary>
   public enum RDomPriority
   {
      None = 0,
      Fallback = 100,
      Normal = 200,
      Top = 300
   }
}
