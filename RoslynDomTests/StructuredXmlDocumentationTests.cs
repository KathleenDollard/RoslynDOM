using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.CSharp;
using RoslynDom.Common;

namespace RoslynDomTests
{
   [TestClass]
   public class StructuredXmlDocumentationTests
   {
      [TestMethod]
      public void Can_load_and_retrieve_structured_comments()
      {
         var csharpCode =
@"                        public class Foo
                        {
                            /// <summary>
                            /// This is a test
                            /// </summary>
                            /// <param name=""dummy"">With a dummy parameter</param>
                            public void Foo3(string dummy)
                            {
                                Console.WriteLine(42);
                            }
                        }";
         VerifyStructuredComments(csharpCode, x => x.Classes.First().Methods.First(), "This is a test");
      }


      [TestMethod]
      public void Can_load_and_retrieve_multiline_structured_comments()
      {
         var csharpCode =
@"                        public class Foo
                        {
                            /// <summary>
                            /// This is a test and another test and another
                            /// and another and another
                            /// </summary>
                            /// <param name=""dummy"">With a dummy parameter</param>
                            public void Foo3(string dummy)
                            {
                                Console.WriteLine(42);
                            }
                        }";
         VerifyStructuredComments(csharpCode, x => x.Classes.First().Methods.First(), 
            "This is a test and another test and another\n    and another and another");
      }

      public void VerifyStructuredComments(string csharpCode, 
                  Func<IRoot,IHasStructuredDocumentation > makeItem,
                  string description)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var root2 = root.Copy();
         var item = makeItem(root);
         var item2 = makeItem(root2);
         Assert.IsNotNull(item2);
         Assert.AreEqual(description, item.Description);
         Assert.AreEqual(description, item2.Description);
         Assert.IsTrue(item.SameIntent(item2));
         var actual1 = RDom.CSharp.GetSyntaxNode(root).ToFullString();
         var actual2 = RDom.CSharp.GetSyntaxNode(root2).ToFullString();
         Assert.AreEqual(csharpCode, actual1);
         Assert.AreEqual(csharpCode, actual2);


      }
   }
}
