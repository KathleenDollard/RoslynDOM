using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   public interface IContainer
   {
      IEnumerable<IDom> GetMembers();
      bool AddOrMoveMember(IDom item);
      bool RemoveMember(IDom item);
      bool InsertOrMoveMember(int index, IDom item);
   }
}
