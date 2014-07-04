using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;

namespace RoslynDomTests
{
    [TestClass]
    public class CopyTests
    {
        private const string SameIntentTestCategory = "SameIntent";
        private const string CopyCategory = "Copy";
        private const string CopyMethodsCategory = "CopyMethods";

        #region clone tests
        [TestMethod,TestCategory(CopyCategory)]
        public void Can_clone_attribute()
        {
            var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2:""George"")] 
            public class Bar{}           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attribute = root.RootClasses.First().Attributes.First();
            var newAttribute = attribute.Copy();
            Assert.IsNotNull(newAttribute);
            Assert.IsTrue(newAttribute.SameIntent(attribute));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_attribute_value()
        {
            var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2:""George"")] 
            public class Bar{}           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var attributeValue = root.RootClasses.First().Attributes.First().AttributeValues.Last();
            var newAttributeValue = attributeValue.Copy();
            Assert.IsNotNull(newAttributeValue);
            Assert.IsTrue(newAttributeValue.SameIntent(attributeValue));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_method()
        {
            var csharpCode = @"
            public class Bar
            {
               public string Foo(int id, string firstName, string lastName) {}
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var method = root.RootClasses.First().Methods.First();
            var newMethod = method.Copy();
            Assert.IsNotNull(newMethod);
            Assert.IsTrue(newMethod.SameIntent(method));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_method_parameter()
        {
            var csharpCode = @"
            public class Bar
            {
               public string Foo(int id, string firstName, string lastName) {}
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var methodParameter = root.RootClasses.First().Methods.First().Parameters .First();
            var newMethodParameter = methodParameter.Copy();
            Assert.IsNotNull(newMethodParameter);
            Assert.IsTrue(newMethodParameter.SameIntent(methodParameter));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_method_body()
        {
            Assert.Inconclusive();
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_property()
        {
            var csharpCode = @"
            public class Bar
            {
               public string Foo{get; set;}
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var property = root.RootClasses.First().Properties.First();
            var newProperty = property.Copy();
            Assert.IsNotNull(newProperty);
            Assert.IsTrue(newProperty.SameIntent(property));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_property_get_body()
        {
            Assert.Inconclusive();
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_property_set_body()
        {
            Assert.Inconclusive();
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_class()
        {
            var csharpCode = @"
            public class Bar
            {
               public string Foo{get; set;}
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var newClass = class1.Copy();
            Assert.IsNotNull(newClass);
            Assert.IsTrue(newClass.SameIntent(class1));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_structure()
        {
            var csharpCode = @"
            public struct Bar
            {
               public string Foo{get; set;}
               public string Foo2(int FooBar) {}
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var structure = root.RootStructures.First();
            var newStructure = structure.Copy();
            Assert.IsNotNull(newStructure);
            Assert.IsTrue(newStructure.SameIntent(structure));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_interface()
        {
            var csharpCode = @"
            public interface Bar
            {
               string Foo{get; set;}
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var interface1 = root.RootInterfaces.First();
            var newInterface = interface1.Copy();
            Assert.IsNotNull(newInterface);
            Assert.IsTrue(newInterface.SameIntent(interface1));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_enum()
        {
            var csharpCode = @"
            public enum Bar
            {
              Unknown, Red, Green, Blue
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var enum1 = root.RootEnums.First();
            var newEnum = enum1.Copy();
            Assert.IsNotNull(newEnum);
            Assert.IsTrue(newEnum.SameIntent(enum1));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_namespace()
        {
            var csharpCode = @"
            namespace Namespace1
            {
                public interface Bar
                {
                    string Foo { get; set; }
                }  
            }         
            ";     
            var root = RDomFactory.GetRootFromString(csharpCode);
            var namespace1 = root.Namespaces.First();
            var newNamespace = namespace1.Copy();
            Assert.IsNotNull(newNamespace);
            Assert.IsTrue(newNamespace.SameIntent(namespace1));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_root()
        {
            var csharpCode = @"
            namespace Namespace1
            {
                public interface Bar
                {
                    string Foo { get; set; }
                }  
            }         
            namespace Namespace2
            {
                public class Bar2
                {
                    string Foo2(int George) {}
                }  
            }         
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var newRoot = root.Copy();
            Assert.IsNotNull(newRoot);
            Assert.IsTrue(newRoot.SameIntent(root));
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Clone_includes_public_annotations()
        {
            var csharpCode = @"
            //[[ kad_Test3(val1 : ""Fred"", val2 : 42) ]]
            public class Bar
            {
               //[[ kad_Test4() ]]
               public string Foo{get; set;}
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var newClass = class1.Copy();
            Assert.IsNotNull(newClass);
            Assert.IsTrue(newClass.SameIntent(class1));
            Assert.AreEqual(42, newClass.PublicAnnotations.GetValue("kad_Test3", "val2"));
        }

        #endregion
    }
}
