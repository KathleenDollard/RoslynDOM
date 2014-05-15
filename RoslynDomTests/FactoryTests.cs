using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;

namespace RoslynDomTests
{
    [TestClass]
    public class FactoryTests
    {
        [TestMethod]
        public void Can_get_root_from_file()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Namespaces.Count());
        }

        [TestMethod]
        public void Can_get_root_from_string()
        {
            var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { }
                        ";
            IRoot root = RDomFactory.GetRootFromString(csharpCode);
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

        #region symbol tests
        [TestMethod]
        public void Can_get_symbol_for_namespace()
        {
            var csharpCode = @"
                        namespace testing.Namespace1
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Namespaces.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("testing.Namespace1", symbol.Name);
        }

        [TestMethod]
        public void Can_get_symbol_for_class()
        {
            var csharpCode = @"
                        public class MyClass
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyClass", symbol.Name);
        }

        [TestMethod]
        public void Can_get_symbol_for_enum()
        {
            var csharpCode = @"
                        public enum MyEnum
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Enums.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyEnum", symbol.Name);
        }

        [TestMethod]
        public void Can_get_symbol_for_struct()
        {
            var csharpCode = @"
                        public struct MyStruct
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Structures.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyStruct", symbol.Name);
        }

        [TestMethod]
        public void Can_get_symbol_for_interface()
        {
            var csharpCode = @"
                        public interface MyInterface
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Interfaces.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyInterface", symbol.Name);
        }


        [TestMethod]
        public void Can_get_symbol_for_field()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myField; }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First().Fields.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("myField", symbol.Name);
        }

        [TestMethod]
        public void Can_get_symbol_for_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myProperty { get; } }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First().Properties.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("myProperty", symbol.Name);
        }

        [TestMethod]
        public void Can_get_symbol_for_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First().Methods.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("myMethod", symbol.Name);
        }

        [TestMethod]
        public void Can_get_symbol_for_nestedType()
        {
            var csharpCode = @"
                        public class MyClass
                        { public class MyNestedClass {  } }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First().Types.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyNestedClass", symbol.Name);
        }

        #endregion
    }
}
