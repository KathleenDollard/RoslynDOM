using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDomCommon
{
   interface IRetrieveValue
   {
   }

   interface IRetrieveValue<T>
   {
      object RetrieveValue(T item, string name);
      string RetrieveString(T item, string name);
      int RerieveInteger(T item, string name);
   }
}
