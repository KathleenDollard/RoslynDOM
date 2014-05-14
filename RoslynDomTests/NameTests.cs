using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;

namespace RoslynDomTests
{
    [TestClass]
    public class NameTests
    {
        [TestMethod]
        public void Root_is_named_root()
        {
            var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("Root", root.Name);
        }

        [TestMethod]
        public void Can_get_namespace_name()
        {
            var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("testing.Namespace1", root.Namespaces.First().Name);
        }

        [TestMethod]
        public void Can_get_class_name()
        {
            var csharpCode = @"
                        public class MyClass
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("MyClass", root.Classes.First().Name);
        }

        [TestMethod]
        public void Can_get_field_name()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_get_property_name()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void Can_get_method_name()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void Can_get_nestedType_name()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_get_nested_namespace_name()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_get_nested_class_name()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_get_nested_field_name()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_get_nested_property_name()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void Can_get_nested_method_name()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void Can_get_nested_nestedType_name()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_get_keyword_namespace_name()
        {
            var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace @class
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("class", root.Namespaces.First().Name);
        }

        [TestMethod]
        public void Can_get_keyword_class_name()
        {
            var csharpCode = @"
                        public class @namespace
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("namespace", root.Classes.First().Name);
        }

        [TestMethod]
        public void Can_get_keyword_field_name()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_get_keyword_property_name()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void Can_get_keyword_method_name()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public void Can_get_keyword_nestedType_name()
        {
            Assert.Inconclusive();
        }
    }
}
