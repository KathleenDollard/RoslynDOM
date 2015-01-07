using System;

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