using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;

namespace RoslynDomTests
{
    [TestClass]
    public class PublicAnnotationTests
    {
        private const string PublicAnnotationsOnRootCategory = "PublicAnnotationsOnRoot";
        private const string PublicAnnotationsOnOtherNodesCategory = "PublicAnnotationsOnOtherNodes";
        private const string PublicAnnotationValuesCategory = "PublicAnnotationValues";
        private const string PublicAnnotationAddingNewCategory = "PublicAnnotationAddingNew";

        #region public annotations on root
        [TestMethod,TestCategory(PublicAnnotationsOnRootCategory)]
        public void Get_public_annotations_on_root_with_using_annotation()
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
            Assert.AreEqual(true, root.PublicAnnotations.HasPublicAnnotation("kad_Test1"));
            Assert.AreEqual(true, root.PublicAnnotations.HasPublicAnnotation("kad_Test2"));
        }

        [TestMethod, TestCategory(PublicAnnotationsOnRootCategory)]
        public void Get_public_root_annotations_on_root()
        {
            var csharpCode = @"
            //[[root:kad_Test1()]]
                    
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsTrue(root.PublicAnnotations.HasPublicAnnotation("kad_Test1"));
        }

        [TestMethod, TestCategory(PublicAnnotationsOnRootCategory)]
        public void Get_public_file_annotations_on_root()
        {
            var csharpCode = @"
            //[[ file:kad_Test2() ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsTrue(root.PublicAnnotations.HasPublicAnnotation("kad_Test2"));
        }

        [TestMethod, TestCategory(PublicAnnotationsOnRootCategory)]
        public void Get_public_annotations_on_root_when_not_first()
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
            Assert.IsTrue(root.PublicAnnotations.HasPublicAnnotation("kad_Test1"));
            Assert.IsTrue(root.PublicAnnotations.HasPublicAnnotation("kad_Test2"));
        }

        [TestMethod, TestCategory(PublicAnnotationsOnRootCategory)]
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
        [TestMethod,TestCategory(PublicAnnotationsOnOtherNodesCategory)]
        public void Get_public_annotations_on_using()
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
            Assert.IsTrue(using1.PublicAnnotations.HasPublicAnnotation("kad_Test3"));
            Assert.IsTrue(using2.PublicAnnotations.HasPublicAnnotation("kad_Test4"));
            Assert.IsTrue(using2.PublicAnnotations.HasPublicAnnotation("kad_Test5"));
        }

        [TestMethod, TestCategory(PublicAnnotationsOnOtherNodesCategory)]
        public void Get_public_annotations_on_namespace()
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
            Assert.IsTrue(namespace1.PublicAnnotations.HasPublicAnnotation("kad_Test4"));
            Assert.IsTrue(namespace1.PublicAnnotations.HasPublicAnnotation("kad_Test5"));

        }

        [TestMethod, TestCategory(PublicAnnotationsOnOtherNodesCategory)]
        public void Get_public_annotations_on_type()
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
            Assert.IsTrue(class1.PublicAnnotations.HasPublicAnnotation("kad_Test6"));
            Assert.IsTrue(structure1.PublicAnnotations.HasPublicAnnotation("kad_Test7"));
            Assert.IsTrue(structure1.PublicAnnotations.HasPublicAnnotation("kad_Test8"));
            Assert.IsTrue(interface1.PublicAnnotations.HasPublicAnnotation("kad_Test9"));
            Assert.IsTrue(interface1.PublicAnnotations.HasPublicAnnotation("kad_TestA"));
            Assert.IsTrue(enum1.PublicAnnotations.HasPublicAnnotation("kad_TestB"));
            Assert.IsTrue(enum1.PublicAnnotations.HasPublicAnnotation("kad_TestC"));
        }

        [TestMethod, TestCategory(PublicAnnotationsOnOtherNodesCategory)]
        public void Get_public_annotations_on_member()
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
            Assert.IsTrue(field.PublicAnnotations.HasPublicAnnotation("kad_Test7"));
            Assert.IsTrue(property.PublicAnnotations.HasPublicAnnotation("kad_Test8"));
            Assert.IsTrue(property.PublicAnnotations.HasPublicAnnotation("kad_Test9"));
            Assert.IsTrue(method.PublicAnnotations.HasPublicAnnotation("kad_TestA"));
        }
        #endregion

        #region public annotation values
        [TestMethod,TestCategory(PublicAnnotationValuesCategory)]
        public void Does_not_crash_when_public_annotations_does_not_exist()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 : ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual(false, root.RootClasses.First().PublicAnnotations.HasPublicAnnotation("xxxx"));
            Assert.IsNull(root.RootClasses.First().PublicAnnotations.GetValue("xxxx", "yyy"));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void Does_notcash_when_public_annotations_value_does_not_exist()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 : ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsNull(root.RootClasses.First().PublicAnnotations.GetValue("kad_Test3", "yyy"));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void Get_public_annotations_values_with_colons()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 : ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("Fred", root.RootClasses.First().PublicAnnotations.GetValue("kad_Test3", "val1"));
            Assert.AreEqual(42, root.RootClasses.First().PublicAnnotations.GetValue<int>("kad_Test3", "val2"));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void Get_public_annotations_values_with_equals()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 = ""Fred"", val2 = 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("Fred", root.RootClasses.First().PublicAnnotations.GetValue("kad_Test3", "val1"));
            Assert.AreEqual(42, root.RootClasses.First().PublicAnnotations.GetValue("kad_Test3", "val2"));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void Get_public_annotations_positional_value()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(""Fred"", val2 = 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("Fred", root.RootClasses.First().PublicAnnotations.GetValue("kad_Test3"));
            Assert.AreEqual("Fred", root.RootClasses.First().PublicAnnotations.GetValue<string>("kad_Test3"));
            Assert.AreEqual(42, root.RootClasses.First().PublicAnnotations.GetValue("kad_Test3", "val2"));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void Get_public_annotations_default_with_bad_name()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(""Fred"", val2 = 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.AreEqual("Fred", root.RootClasses.First().PublicAnnotations.GetValue("kad_Test3"));
            Assert.AreEqual(null, root.RootClasses.First().PublicAnnotations.GetValue("kad_TestX", "val2"));
            Assert.AreEqual(0, root.RootClasses.First().PublicAnnotations.GetValue<int>("kad_TestX", "val2"));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void Same_intent_true_when_public_annotations_match()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(""Fred"", val3 = 3, val2 = 42) ]]
            public class MyClass
            { }
            //[[ kad_Test3(""Fred"", val2 = 42, val3 = 3) ]]
            public class MyClass2
            { }
               ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var class2 = root.RootClasses.Last();
            Assert.IsTrue(class1.PublicAnnotations.SameIntent(class2.PublicAnnotations ));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void Same_intent_false_when_not_matching()
        {
            var csharpCode = @"
            //[[ kad_Test3(""Fred"", val3 = 3, val2 = 42) ]]
            public class MyClass
            { }
            //[[ kad_Test3(""Fred"", val2 = 42, val3 = 4) ]]
            public class MyClass2
            { }
            //[[ kad_Test3(""Fred"", val2 = 42) ]]
            public class MyClass3
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var classes = root.RootClasses.ToArray();
            Assert.IsFalse(classes[0].PublicAnnotations.SameIntent(classes[1].PublicAnnotations));
            Assert.IsFalse(classes[0].PublicAnnotations.SameIntent(classes[2].PublicAnnotations));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void Same_intent_false_with_SameIntent_values_not_matching()
        {
            var csharpCode = @"
            //[[ kad_Test3(""Fred"", val3 = 3, val2 = 42) ]]
            public class MyClass
            { }
            //[[ kad_Test3(""Fred"", val3 = 3, val2 = 42) ]]
            public class MyClass2
            { }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var classes = root.RootClasses.ToArray();
            var class1 = classes[0].Copy();
            var class2 = classes[1].Copy();
            classes[0].PublicAnnotations.AddValue("test4", class1);
            classes[1].PublicAnnotations.AddValue("test4", class2);
            Assert.IsFalse(classes[0].PublicAnnotations.SameIntent(classes[1].PublicAnnotations));
        }
        #endregion

        #region public annotation adding new
        [TestMethod, TestCategory(PublicAnnotationAddingNewCategory)]
        public void Can_add_public_annotation_with_key()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(""Fred"", val2 = 42) ]]
            public class MyClass
            { }
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var cl = root.RootClasses.First();
            var newName = "kad_Test3";
            var newKey = "val3";
            var newValue = 43;
            cl.PublicAnnotations.AddValue(newName, newKey, newValue);
            Assert.AreEqual("Fred", cl.PublicAnnotations.GetValue("kad_Test3"));
            Assert.AreEqual(42, cl.PublicAnnotations.GetValue("kad_Test3", "val2"));
            Assert.AreEqual(newValue , cl.PublicAnnotations.GetValue(newName, newKey));
        }

        [TestMethod, TestCategory(PublicAnnotationAddingNewCategory)]
        public void Can_add_public_annotation_without_key()
        {
            var csharpCode = @"
            using Foo;
                     
            public class MyClass
            { }
";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var cl = root.RootClasses.First();
            var newName = "kad_Test3";
            var newValue = 43;
            cl.PublicAnnotations.AddValue(newName,  newValue);
            Assert.AreEqual(newValue, cl.PublicAnnotations.GetValue(newName));
        }
        #endregion

    }
}
