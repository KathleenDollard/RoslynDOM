using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RoslynDomTests
{
   [TestClass]
   public class CSharpSpecTests
   {
      private const string MutabilityCategory = "Mutability";

      [TestMethod]
      public void TestMethod1()
      {
         var @afsadf = "";
         Console.WriteLine(afsadf);
      }
   }
}
