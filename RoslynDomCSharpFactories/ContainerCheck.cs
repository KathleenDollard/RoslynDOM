using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   public class ContainerChecker : IContainerCheck
   {
      public bool ContainerCheck()
      {
         return RDom.CSharp.ContainerCheck();
      }
   }
}