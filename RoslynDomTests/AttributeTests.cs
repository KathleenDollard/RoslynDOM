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
    public class AttributeTests
    {
        #region get attributes
        [TestMethod]
        public void Can_get_attributes_on_class()
        {
            var csharpCode = @"
                        [Serializable]                        
                        public class MyClass
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Attributes;
            Assert.AreEqual(1,attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
        }

        [TestMethod]
        public void Can_get_attributes_on_enum()
        {
            var csharpCode = @"
                        [Serializable]                        
                        public enum MyEnum
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Enums.First().Attributes;
            Assert.AreEqual(1, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
        }

        [TestMethod]
        public void Can_get_attributes_on_struct()
        {
            var csharpCode = @"
                        [Serializable]                        
                        public struct MyStruct
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Structures .First().Attributes;
            Assert.AreEqual(1, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
        }

        [TestMethod]
        public void Can_get_attributes_on_interface()
        {
            var csharpCode = @"
                        [Serializable]                        
                        public interface MyInterface
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Interfaces.First().Attributes;
            Assert.AreEqual(1, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
        }

 
        [TestMethod]
        public void Can_get_attributes_on_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { [Serializable] public int myMethod(int x) { return x; } }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Methods.First().Attributes;
            Assert.AreEqual(1, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
        }

        [TestMethod]
        public void Can_get_attributes_on_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { [Serializable]  public int myProperty { get; } }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Properties.First().Attributes;
            Assert.AreEqual(1, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
        }

        [TestMethod]
        public void Can_get_attributes_on_field()
        {
            Assert.Inconclusive();
        }
        #endregion

        #region get multiple attributes one to a bracket set
        [TestMethod]
        public void Can_get_multiple_attributes_in_separate_brackets_on_class()
        {
            var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public class MyClass
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_attributes_in_separate_brackets_on_enum()
        {
            var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public enum MyEnum
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Enums.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_attributes_in_separate_brackets_on_struct()
        {
            var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public struct MyStruct
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Structures.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_attributes_in_separate_brackets_on_interface()
        {
            var csharpCode = @"
                        [Serializable]
                        [TestClass]                        
                        public interface MyInterface
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Interfaces.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

         [TestMethod]
        public void Can_get_multiple_attributes_in_separate_brackets_on_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable] 
                        [TestClass]                        
                        public int myMethod(int x) { return x; } }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Methods.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_attributes_in_separate_brackets_on_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable] 
                        [TestClass]                        
                        public int myProperty { get; }  }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Properties.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_attributes_in_separate_brackets_on_field()
        {
            Assert.Inconclusive();
        }

        #endregion

        #region get multiple attributes many in bracket
        [TestMethod]
        public void Can_get_multiple_attributes_in_shared_brackets_on_class()
        {
            var csharpCode = @"
                        [Serializable, TestClass]                        
                        public class MyClass
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_attributes_in_shared_brackets_on_enum()
        {
            var csharpCode = @"
                        [Serializable, TestClass]                        
                        public enum MyEnum
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Enums.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_attributes_in_shared_brackets_on_struct()
        {
            var csharpCode = @"
                        [Serializable, TestClass]                        
                        public struct MyStruct
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Structures.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_attributes_in_shared_brackets_on_interface()
        {
            var csharpCode = @"
                        [Serializable, TestClass]                        
                        public interface MyInterface
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Interfaces.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

  
        [TestMethod]
        public void Can_get_multiple_attributes_in_shared_brackets_on_method()
        {
            var csharpCode = @"
                         public class MyClass
                        { 
                        [Serializable] 
                        [TestClass]                        
                          public int myMethod(int x) { return x; } }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Methods.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_attributes_in_shared_brackets_on_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable] 
                        [TestClass]                        
                         public int myProperty { get; } }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Properties.First().Attributes;
            Assert.AreEqual(2, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_attributes_in_shared_brackets_on_field()
        {
            Assert.Inconclusive();
        }
        #endregion


        #region get multiple attributes with mixed bracketing
        [TestMethod]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_class()
        {
            var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public class MyClass
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Attributes;
            Assert.AreEqual(3, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_enum()
        {
            var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public enum MyEnum
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Enums.First().Attributes;
            Assert.AreEqual(3, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_struct()
        {
            var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public struct MyStruct
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Structures.First().Attributes;
            Assert.AreEqual(3, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_interface()
        {
            var csharpCode = @"
                        [Serializable, TestClass]
                        [Ignore]                  
                        public interface MyInterface
                            { }
                        ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Interfaces.First().Attributes;
            Assert.AreEqual(3, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { 
                        [Serializable, TestClass]
                        [Ignore]                  
                        public int myMethod(int x) { return x; } }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Methods.First().Attributes;
            Assert.AreEqual(3, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_property()
        {
            var csharpCode = @"
                        public class MyClass
                       { 
                        [Serializable, TestClass]
                        [Ignore]                  
                        public int myProperty { get; } }";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributes = root.Classes.First().Properties.First().Attributes;
            Assert.AreEqual(3, attributes.Count());
            Assert.AreEqual("Serializable", attributes.First().Name);
            Assert.AreEqual("TestClass", attributes.Skip(1).First().Name);
            Assert.AreEqual("Ignore", attributes.Last().Name);
        }

        [TestMethod]
        public void Can_get_multiple_multiple_attributes_with_mixed_brackets_in_shared_brackets_on_field()
        {
            Assert.Inconclusive();
        }
        #endregion
    }
}
