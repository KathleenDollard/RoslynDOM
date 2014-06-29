using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;

namespace RoslynDomTests
{
    [TestClass]
    public class PublicAnnotationTests
    {
        private const string PublicAnnotationsOnRootCategory = "PublicAnnotationsOnRoot";
        private const string PublicAnnotationsOnOtherNodesCategory = "PublicAnnotationsOnOtherNodes";
        private const string PublicAnnotationValuesCategory = "PublicAnnotationValues";

        #region public annotations on root
        [TestMethod]
        [TestCategory(PublicAnnotationsOnRootCategory)]
        public void Can_get_public_annotations_on_root_with_using_annotation()
        {
            var csharpCode = @"
            //[[ root:kad_Test1() ]]
            //[[ file:kad_Test2() ]]
            //[[ kad_Test3() ]]
            using Foo;
                     
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual(true, root.HasPublicAnnotation("kad_Test1"));
            Assert.AreEqual(true, root.HasPublicAnnotation("kad_Test2"));
        }

        [TestMethod]
        [TestCategory(PublicAnnotationsOnRootCategory)]
        public void Can_get_public_root_annotations_on_root()
        {
            var csharpCode = @"
            //[[root:kad_Test1()]]
                    
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsTrue(root.HasPublicAnnotation("kad_Test1"));
    }

    [TestMethod]
        [TestCategory(PublicAnnotationsOnRootCategory)]
        public void Can_get_public_file_annotations_on_root()
        {
            var csharpCode = @"
            //[[ file:kad_Test2() ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsTrue(root.HasPublicAnnotation("kad_Test2"));
        }

        [TestMethod]
        [TestCategory(PublicAnnotationsOnRootCategory)]
        public void Can_get_public_annotations_on_root_when_not_first()
        {
            var csharpCode = @"
            //[[ kad_Test3() ]]
            using Foo;
                     
            //[[ root:kad_Test1() ]]
            //[[ file:kad_Test2() ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsTrue(root.HasPublicAnnotation("kad_Test1"));
            Assert.IsTrue(root.HasPublicAnnotation("kad_Test2"));
        }

        [TestMethod]
        [TestCategory(PublicAnnotationsOnRootCategory)]
        public void Does_not_fail_with_no_root_annotations()
        {
            var csharpCode = @"
            //[[ kad_Test3() ]]
            using Foo;
                     
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
        }
        #endregion

        #region public annotation on other nodes
        [TestMethod]
        [TestCategory(PublicAnnotationsOnOtherNodesCategory)]
        public void Can_get_public_annotations_on_using()
        {
            var csharpCode = @"
            //[[ root:kad_Test1() ]]
            //[[ file:kad_Test2() ]]
            //[[ kad_Test3() ]]
            using Foo;
                     
            //[[ kad_Test4() ]]
            //[[ kad_Test5() ]]
            using Foo2;

            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var using1 = root.Usings.First();
            var using2 = root.Usings.Last();
            Assert.IsTrue(using1.HasPublicAnnotation("kad_Test3"));
            Assert.IsTrue(using2.HasPublicAnnotation("kad_Test4"));
            Assert.IsTrue(using2.HasPublicAnnotation("kad_Test5"));
        }

        [TestMethod]
        [TestCategory(PublicAnnotationsOnOtherNodesCategory)]
        public void Can_get_public_annotations_on_namespace()
        {
            var csharpCode = @"
            //[[ root:kad_Test1() ]]
            //[[ file:kad_Test2() ]]
            //[[ kad_Test3() ]]
            using Foo;
            
            //[[ kad_Test4() ]]
            //[[ kad_Test5() ]]
            namespace Namespace1
            {         
                //[[ kad_Test6() ]]
                public class MyClass
                { }
            }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var namespace1 = root.Namespaces.First();
            Assert.IsTrue(namespace1.HasPublicAnnotation("kad_Test4"));
            Assert.IsTrue(namespace1.HasPublicAnnotation("kad_Test5"));

        }

        [TestMethod]
        [TestCategory(PublicAnnotationsOnOtherNodesCategory)]
        public void Can_get_public_annotations_on_type()
        {
            var csharpCode = @"
            //[[ root:kad_Test1() ]]
            //[[ file:kad_Test2() ]]
            //[[ kad_Test3() ]]
            using Foo;
            
            //[[ kad_Test4() ]]
            //[[ kad_Test5() ]]
            namespace Namespace1
            {         
                //[[ kad_Test6() ]]
                public class MyClass
                { }
 
                //[[ kad_Test7() ]]
                //[[ kad_Test8() ]]
                public struct MyStructure
                { }

                //[[ kad_Test9() ]]
                //[[ kad_TestA() ]]
                public interface MyInterface
                { }

                //[[ kad_TestB() ]]
                //[[ kad_TestC() ]]
                public enum MyEnum
                { }
}
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var structure1 = root.RootStructures.First();
            var interface1 = root.RootInterfaces.First();
            var enum1 = root.RootEnums.First();
            Assert.IsTrue(class1.HasPublicAnnotation("kad_Test6"));
            Assert.IsTrue(structure1.HasPublicAnnotation("kad_Test7"));
            Assert.IsTrue(structure1.HasPublicAnnotation("kad_Test8"));
            Assert.IsTrue(interface1.HasPublicAnnotation("kad_Test9"));
            Assert.IsTrue(interface1.HasPublicAnnotation("kad_TestA"));
            Assert.IsTrue(enum1.HasPublicAnnotation("kad_TestB"));
            Assert.IsTrue(enum1.HasPublicAnnotation("kad_TestC"));
        }

        [TestMethod]
        [TestCategory(PublicAnnotationsOnOtherNodesCategory)]
        public void Can_get_public_annotations_on_member()
        {
            var csharpCode = @"
            //[[ root:kad_Test1() ]]
            //[[ file:kad_Test2() ]]
            //[[ kad_Test3() ]]
            using Foo;
            
            //[[ kad_Test4() ]]
            //[[ kad_Test5() ]]
            namespace Namespace1
            {         
                //[[ kad_Test6() ]]
                public class MyClass
                {
                    //[[ kad_Test7() ]]
                    public string Foo3;
                    //[[ kad_Test8() ]]
                    //[[ kad_Test9() ]]
                    public string Foo4{get;}
                    //[[ kad_TestA() ]]
                    public string Foo5() {}
                }
 
                //[[ kad_Test7() ]]
                //[[ kad_Test8() ]]
                public class MyClass
                { }
}
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var field = root.RootClasses.First().Fields.First();
            var property = root.RootClasses.First().Properties.First();
            var method = root.RootClasses.First().Methods.First();
            Assert.IsTrue(field.HasPublicAnnotation("kad_Test7"));
            Assert.IsTrue(property.HasPublicAnnotation("kad_Test8"));
            Assert.IsTrue(property.HasPublicAnnotation("kad_Test9"));
            Assert.IsTrue(method.HasPublicAnnotation("kad_TestA"));
        }
        #endregion

        #region public annotation values
        [TestMethod]
        [TestCategory(PublicAnnotationValuesCategory)]
        public void Does_notcash_when_public_annotations_does_not_exist()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 : ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual(false, root.RootClasses.First().HasPublicAnnotation("xxxx"));
            Assert.IsNull(root.RootClasses.First().GetPublicAnnotationValue("xxxx", "yyy"));
        }

        [TestMethod]
        [TestCategory(PublicAnnotationValuesCategory)]
        public void Does_notcash_when_public_annotations_value_does_not_exist()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 : ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsNull(root.RootClasses.First().GetPublicAnnotationValue("kad_Test3", "yyy"));
        }

        [TestMethod]
        [TestCategory(PublicAnnotationValuesCategory)]
        public void Can_get_public_annotations_values_with_colons()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 : ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("Fred", root.RootClasses.First().GetPublicAnnotationValue("kad_Test3", "val1"));
            Assert.AreEqual(42, root.RootClasses.First().GetPublicAnnotationValue("kad_Test3", "val2"));
        }

        [TestMethod]
        [TestCategory(PublicAnnotationValuesCategory)]
        public void Can_get_public_annotations_values_with_equals()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 = ""Fred"", val2 = 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("Fred", root.RootClasses.First().GetPublicAnnotationValue("kad_Test3", "val1"));
            Assert.AreEqual(42, root.RootClasses.First().GetPublicAnnotationValue("kad_Test3", "val2"));
        }

        [TestMethod]
        [TestCategory(PublicAnnotationValuesCategory)]
        public void Can_get_public_annotations_positional_value()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(""Fred"", val2 = 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("Fred", root.RootClasses.First().GetPublicAnnotationValue("kad_Test3"));
            Assert.AreEqual(42, root.RootClasses.First().GetPublicAnnotationValue("kad_Test3", "val2"));
        }
        #endregion

    }
}
