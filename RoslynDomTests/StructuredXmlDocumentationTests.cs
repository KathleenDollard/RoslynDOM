using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.CSharp;

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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var root2 = root.Copy();
            var method = root.Classes.First().Methods.First();
            var method2 = root2.Classes.First().Methods.First();
            Assert.IsNotNull(method);
            var expected = "This is a test";
            Assert.AreEqual(expected, method.Description);
            Assert.AreEqual(expected, method2.Description);
            Assert.IsTrue(method.SameIntent(method2));
            var actual1 = RDomCSharp.Factory.BuildSyntax(root).ToFullString();
            var actual2 = RDomCSharp.Factory.BuildSyntax(root2).ToFullString();
            Assert.AreEqual(csharpCode, actual1);
            Assert.AreEqual(csharpCode, actual2);

        }
    }
}
