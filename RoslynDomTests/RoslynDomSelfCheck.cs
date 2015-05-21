using System;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
   [TestClass]
   public class RoslynDomSelfCheck
   {
      [TestMethod]
      public void Can_load_RoslynDom_implementations()
      {
         Assert.Inconclusive();
         VerifyFilesInDirectory(@"..\..\..\RoslynDom\Implementations");
         VerifyFilesInDirectory(@"..\..\..\RoslynDom\BasesAndBaseHelpers");
         VerifyFilesInDirectory(@"..\..\..\RoslynDom\StatementImplementations");
         VerifyFilesInDirectory(@"..\..\..\RoslynDom");
      }

      private static void VerifyFilesInDirectory(string dirName)
      {
         var files = Directory.GetFiles(dirName, "*.cs");
         foreach (var file in files)
         {
            var csharpCode = File.ReadAllText(file);
            var root = RDom.CSharp.Load(csharpCode);
            var actual = RDom.CSharp.GetSyntaxNode(root);
            Assert.IsNotNull(root);
            //if (actual.ToFullString() != csharpCode)
            //{
            //   File.WriteAllText(file + ".actual", actual.ToFullString());
            //}
            Assert.AreEqual(csharpCode, actual.ToFullString());
         }
      }

      [TestMethod]
      public void Can_load_specific_RoslynDom_implementation()
      {
         var dirName = @"..\..\..\RoslynDom\Implementations";
         var file = dirName + @"\RDomAttributeValue.cs";
         var csharpCode = File.ReadAllText(file);
         var root = RDom.CSharp.Load(csharpCode);
         var actual = RDom.CSharp.GetSyntaxNode(root);
         var actualString = actual.ToFullString();
         Assert.IsNotNull(root);
         if (actualString != csharpCode)
         {
            File.WriteAllText(file + ".actual", actualString);
         }
         // Assert.AreEqual(csharpCode, actual.ToFullString());
      }
   }
}
