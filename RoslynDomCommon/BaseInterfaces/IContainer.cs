using System.Collections.Generic;

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