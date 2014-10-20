using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.CSharp
{
   public class UpdateRefactoringAttribute : RefactoringAttribute
   {
      public UpdateRefactoringAttribute(string id) : base(id) { }
   }
}
