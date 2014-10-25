using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   public class OutputContext
   {
      public OutputContext(RDomCorporation corporation)
      {
         Corporation = corporation;
      }

      public  RDomCorporation Corporation { get; private set; }
   }
}
