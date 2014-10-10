using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass]
    public class PublicAnnotationTests
    {
        private const string PublicAnnotationsOnRootCategory = "PublicAnnotationsOnRoot";
        private const string PublicAnnotationsOnOtherNodesCategory = "PublicAnnotationsOnOtherNodes";
        private const string PublicAnnotationValuesCategory = "PublicAnnotationValues";
        private const string PublicAnnotationAddingAndCopyingCategory = "PublicAnnotationAddingAndCopying";
        private const string SameIntentPublicAnnotationCategory = "SameIntentPublicAnnotation";

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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
        }
        #endregion

        #region public annotation on other nodes
        [TestMethod,TestCategory(PublicAnnotationsOnOtherNodesCategory)]
        public void Get_public_annotations_on_using_directive()
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var using1 = root.UsingDirectives.First();
            var using2 = root.UsingDirectives.Last();
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual("Fred", root.RootClasses.First().PublicAnnotations.GetValue("kad_Test3", ""));
            Assert.AreEqual("Fred", root.RootClasses.First().PublicAnnotations.GetValue<string>("kad_Test3", ""));
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual("Fred", root.RootClasses.First().PublicAnnotations.GetValue("kad_Test3", ""));
            Assert.AreEqual(null, root.RootClasses.First().PublicAnnotations.GetValue("kad_TestX", "val2"));
            Assert.AreEqual(0, root.RootClasses.First().PublicAnnotations.GetValue<int>("kad_TestX", "val2"));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void TryGetValue_public_annotation_list()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 = ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            object value1;
            object value2;
            object valueX;
            string value1String;
            int value2Int;
            int valueXInt;
            IPublicAnnotation annotValue;
            IPublicAnnotation annotValue2;
            var publicAnnotations = root.RootClasses.First().PublicAnnotations;
            Assert.IsTrue(publicAnnotations.TryGetValue("kad_Test3","val1", out value1));
            Assert.IsTrue(publicAnnotations.TryGetValue("kad_Test3", "val2", out value2));
            Assert.IsTrue(publicAnnotations.TryGetValue<string>("kad_Test3", "val1", out value1String));
            Assert.IsTrue(publicAnnotations.TryGetValue<int>("kad_Test3", "val2", out value2Int));
            Assert.IsTrue(publicAnnotations.TryGetPublicAnnotation("kad_Test3", out annotValue));
            Assert.IsFalse(publicAnnotations.TryGetValue("kad_Test3", "valX", out valueX));
            Assert.IsFalse(publicAnnotations.TryGetValue<int>("kad_Test3", "valX", out valueXInt));
            Assert.IsFalse(publicAnnotations.TryGetPublicAnnotation("kad_Test3X", out annotValue2));
            Assert.AreEqual("Fred", value1);
            Assert.AreEqual("Fred", value1String);
            Assert.AreEqual(42, value2);
            Assert.AreEqual(42, value2Int);
            Assert.IsNotNull(annotValue);
            Assert.IsNull(annotValue2);
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void TryGetValue_public_annotation()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 = ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            object value1;
            object value2;
            object valueX;
            string value1String;
            int value2Int;
            int valueXInt;
            var publicAnnotation = root.RootClasses.First().PublicAnnotations.GetPublicAnnotation("kad_Test3");
            Assert.IsTrue(publicAnnotation.TryGetValue( "val1", out value1));
            Assert.IsTrue(publicAnnotation.TryGetValue( "val2", out value2));
            Assert.IsTrue(publicAnnotation.TryGetValue<string>( "val1", out value1String));
            Assert.IsTrue(publicAnnotation.TryGetValue<int>( "val2", out value2Int));
            Assert.IsFalse(publicAnnotation.TryGetValue( "valX", out valueX));
            Assert.IsFalse(publicAnnotation.TryGetValue<int>( "valX", out valueXInt));
            Assert.AreEqual("Fred", value1);
            Assert.AreEqual("Fred", value1String);
            Assert.AreEqual(42, value2);
            Assert.AreEqual(42, value2Int);
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void Same_intent_returns_true_if_public_annotations_check_not_requested()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 = ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var root2 = root.Copy();
            var class1 =root.Classes.First();
            var class2 = root2.Classes.First();
             class2.PublicAnnotations.AddValue("kad_Test4", "George");
            Assert.IsFalse(class1.PublicAnnotations.SameIntent(class2.PublicAnnotations, false));
            Assert.IsTrue(class1.PublicAnnotations.SameIntent(class2.PublicAnnotations,true));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
       [ExpectedException(typeof(InvalidOperationException))]
        public void Same_intent_throws_with_null_annotations()
        {
           var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 = ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
           var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
           var root2 = root.Copy();
           var class1 = root.Classes.First();
           Assert.IsFalse(class1.PublicAnnotations.SameIntent(null));
        }

       [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void HasValue_public_annotations()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 = ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var publicAnnotations = root.RootClasses.First().PublicAnnotations;
            Assert.IsTrue(publicAnnotations.HasValue("kad_Test3", "val1"));
            Assert.IsTrue(publicAnnotations.HasValue("kad_Test3", "val2"));
            Assert.IsTrue(publicAnnotations.HasPublicAnnotation("kad_Test3"));
            Assert.IsFalse(publicAnnotations.HasValue("kad_Test3X", "val2"));
            Assert.IsFalse(publicAnnotations.HasValue("kad_Test3", "val2X"));
            Assert.IsFalse(publicAnnotations.HasPublicAnnotation("kad_Test3X"));
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        public void Public_annotations_accessed_through_interface()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 = ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var publicAnnotations = root.RootClasses.First().PublicAnnotations as IHasLookupValue;
            PublicAnnotation annotValue;
            PublicAnnotation annotValue2;
            var value = publicAnnotations.GetValue("kad_Test3");
            var value2 = publicAnnotations.GetValue<PublicAnnotation>("kad_Test3");
            Assert.IsTrue(publicAnnotations.HasValue("kad_Test3"));
            Assert.IsTrue(publicAnnotations.TryGetValue("kad_Test3", out annotValue));
            Assert.IsFalse(publicAnnotations.TryGetValue("kad_Test3x", out annotValue2));
            Assert.IsNotNull(value);
            Assert.IsNotNull(annotValue);
            Assert.IsNull(annotValue2);
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        [ExpectedException (typeof(InvalidOperationException))]
        public void Public_annotations_through_interface_generic_GetValue_throws_on_bad_type()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 = ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var publicAnnotations = root.RootClasses.First().PublicAnnotations as IHasLookupValue;
            var value = publicAnnotations.GetValue<string>("kad_Test3");
        }

        [TestMethod, TestCategory(PublicAnnotationValuesCategory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Public_annotations_through_interface_generic_TryGetValue_throws_on_bad_type()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 = ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            string annotValue;
            var publicAnnotations = root.RootClasses.First().PublicAnnotations as IHasLookupValue;
            var value = publicAnnotations.TryGetValue<string >("kad_Test3", out annotValue);
        }



        #endregion

        #region same intent public annotation
        [TestMethod, TestCategory(SameIntentPublicAnnotationCategory)]
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var class2 = root.RootClasses.Last();
            Assert.IsTrue(class1.PublicAnnotations.SameIntent(class2.PublicAnnotations ));
        }

        [TestMethod, TestCategory(SameIntentPublicAnnotationCategory)]
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var classes = root.RootClasses.ToArray();
            Assert.IsFalse(classes[0].PublicAnnotations.SameIntent(classes[1].PublicAnnotations));
            Assert.IsFalse(classes[0].PublicAnnotations.SameIntent(classes[2].PublicAnnotations));
            Assert.IsFalse(classes[0].PublicAnnotations.SameIntent(classes[2].PublicAnnotations, false));
            Assert.IsTrue(classes[0].PublicAnnotations.SameIntent(classes[2].PublicAnnotations, true));
        }

        [TestMethod, TestCategory(SameIntentPublicAnnotationCategory)]
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var classes = root.RootClasses.ToArray();
            var class1 = classes[0].Copy();
            var class2 = classes[1].Copy();
            classes[0].PublicAnnotations.AddValue("test4", class1);
            classes[1].PublicAnnotations.AddValue("test4", class2);
            Assert.IsFalse(classes[0].PublicAnnotations.SameIntent(classes[1].PublicAnnotations));
        }
        #endregion

        #region public annotation adding and copying
        [TestMethod, TestCategory(PublicAnnotationAddingAndCopyingCategory)]
        public void Can_add_public_annotation_with_key()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(""Fred"", val2 = 42) ]]
            public class MyClass
            { }
";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.RootClasses.First();
            var newName = "kad_Test3";
            var newKey = "val3";
            var newValue = 43;
            cl.PublicAnnotations.AddValue(newName, newKey, newValue);
            Assert.AreEqual("Fred", cl.PublicAnnotations.GetValue("kad_Test3",""));
            Assert.AreEqual(42, cl.PublicAnnotations.GetValue("kad_Test3", "val2"));
            Assert.AreEqual(newValue , cl.PublicAnnotations.GetValue(newName, newKey));
        }

        [TestMethod, TestCategory(PublicAnnotationAddingAndCopyingCategory)]
        public void Can_add_public_annotation_without_key()
        {
            var csharpCode = @"
            using Foo;
                     
            public class MyClass
            { }
";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.RootClasses.First();
            var newName = "kad_Test3";
            var newValue = 43;
            cl.PublicAnnotations.AddValue(newName,  newValue);
            Assert.AreEqual(newValue, cl.PublicAnnotations.GetValue(newName, newName));
        }

        [TestMethod, TestCategory(PublicAnnotationAddingAndCopyingCategory)]
        public void Public_annotation_add_doesnt_throw_on_null()
        {
            var csharpCode = @"
            using Foo;
                     
            public class MyClass
            { }
";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.RootClasses.First();
            cl.PublicAnnotations.Add(null);
            Assert.IsNotNull( cl.PublicAnnotations);
        }

        [TestMethod, TestCategory(PublicAnnotationAddingAndCopyingCategory)]
        public void Can_copy_public_annotations()
        {
            var csharpCode = @"
            using Foo;
                     
            //[[ kad_Test3(val1 : ""Fred"", val2 : 42) ]]
            public class MyClass
            { }
";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.RootClasses.First();
            var publicAnnotations = cl.PublicAnnotations;
            var newPublicAnnotations = publicAnnotations.Copy();
            Assert.AreEqual(1, newPublicAnnotations.Count());
            Assert.AreEqual("Fred", newPublicAnnotations.First().GetValue("val1"));
            Assert.AreEqual(42, newPublicAnnotations.First().GetValue("val2"));
        }

        [TestMethod, TestCategory(PublicAnnotationAddingAndCopyingCategory)]
        public void Copy_public_annotations_doesnt_throw_when_no_annotations()
        {
            var csharpCode = @"
            using Foo;
                     
            public class MyClass
            { }
";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var cl = root.RootClasses.First();
            var publicAnnotations = cl.PublicAnnotations;
            var newPublicAnnotations = publicAnnotations.Copy();
            Assert.AreEqual(0, newPublicAnnotations.Count());
        }
        #endregion

    }
}
