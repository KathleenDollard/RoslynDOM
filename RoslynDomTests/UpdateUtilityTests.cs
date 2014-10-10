using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;
using RoslynDom.CSharp;
using RoslynDom;
using System.Linq;
using System.IO;

namespace RoslynDomTests
{
   [TestClass]
   public class UpdateUtilityTests
   {
      [TestMethod]
      public void Write_all_text_writes_root()
      {
         const string fileName = "temp.cs";
         var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace Namespace1
                        { }";
         IRoot root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         Assert.IsNotNull(root);
         UpdateUtilities.WriteRootToFile(fileName, root);
         File.Exists(fileName);
         var actual = File.ReadAllText(fileName);
         Assert.AreEqual(csharpCode, actual);
      }

      [TestMethod]
      public void Write_all_text_writes_root_to_new_directory()
      {
         const string dirName = "TempTestDir";
         string fileName = Path.Combine(dirName, "temp.cs");
         if (Directory.Exists(dirName)) Directory.Delete(dirName, true);
         var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace Namespace1
                        { }";
         IRoot root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         Assert.IsNotNull(root);
         UpdateUtilities.WriteRootToFile(fileName, root);
         File.Exists(fileName);
         var actual = File.ReadAllText(fileName);
         Assert.AreEqual(csharpCode, actual);
      }
   }
}
