using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;

namespace RoslynDomTests
{
    [TestClass]
    public class FactoryTests
    {
        [TestMethod]
        public void Can_get_root_from_file()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_get_root_from_string()
        {
            var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Namespaces.Count());
        }

        [TestMethod]
        public void Can_get_root_from_syntaxtree()
        {
            var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { }
                        ";
            var tree = CSharpSyntaxTree.ParseText(csharpCode);
            var root = RDomFactory.GetRootFromSyntaxTree(tree);
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Namespaces.Count());
        }

        [TestMethod]
        public void Can_get_root_from_string_with_invalid_code()
        {
            var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { 
                            publi classX x {}
                            }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Namespaces.Count());
        }


    }
}
