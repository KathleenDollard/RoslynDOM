using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.CSharp
{
   public class RefactoringAttribute : Attribute
   {
      private string _id;

      public RefactoringAttribute(string id)
      {
         _id = id;
      }

      public string Id
      { get { return _id; } }
   }
}
