using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass]
    public class SameIntentTests
    {
        private const string SameIntentAtrributesCategory = "SameIntentAttributes";
        private const string SameIntentMethodCategory = "SameIntentMethods";
        private const string SameIntentPropertyCategory = "SameIntentProperties";
        private const string SameIntentFieldCategory = "SameIntentFields";
        private const string SameIntentClassCategory = "SameIntentClass";
        private const string SameIntentStructureCategory = "SameIntentStructure";
        private const string SameIntentInterfaceCategory = "SameIntentInterface";
        private const string SameIntentNamespaceCategory = "SameIntentNamespace";
        private const string SameIntentRootCategory = "SameIntentRoot";

        #region same intent attributes
        [TestMethod, TestCategory(SameIntentAtrributesCategory)]
        public void Same_intent_true_with_same_attributes()
        {
            var csharpCode1 = @"
            [Foo(""Fred"", bar:3, bar2:""Sam"")]
            public class Class1 {}
            ";
            var csharpCode2 = @"
            [Foo(""Fred"", bar:3, bar2:""Sam"")]
            public struct Struct1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var class1 = root1.Classes.First();
            var struct1 = root2.Structures.First();
            var attribute1 = class1.Attributes.Attributes.First();
            var attribute2 = struct1.Attributes.Attributes.First();
            Assert.IsTrue(attribute1.SameIntent(attribute2));

        }

        [TestMethod, TestCategory(SameIntentAtrributesCategory)]
        public void Same_intent_true_with_attributes_stated_differently()
        {
            var csharpCode1 = @"
            [Foo(""Fred"", bar:3, bar2:""Sam"")]
            public class Class1 {}
            ";
            var csharpCode2 = @"
            [Foo(""Fred"", bar2=""Sam"", bar=3)]
            public struct Struct1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var class1 = root1.Classes.First();
            var struct1 = root2.Structures.First();
            var attribute1 = class1.Attributes.Attributes.First();
            var attribute2 = struct1.Attributes.Attributes.First();
            Assert.IsTrue(attribute1.SameIntent(attribute2));

        }
        
        [TestMethod, TestCategory(SameIntentAtrributesCategory)]
        public void Same_intent_false_with_attributes_with_different_names()
        {
            var csharpCode1 = @"
            [Foo(""Fred"", bar:3, bar2:""Sam"")]
            public class Class1 {}
            ";
            var csharpCode2 = @"
            [Foo2(""Fred"", bar2=""Sam"", bar=3)]
            public struct Struct1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var class1 = root1.Classes.First();
            var struct1 = root2.Structures.First();
            var attribute1 = class1.Attributes.Attributes.First();
            var attribute2 = struct1.Attributes.Attributes.First();
            Assert.IsFalse(attribute1.SameIntent(attribute2));

        }

        [TestMethod, TestCategory(SameIntentAtrributesCategory)]
        public void Same_intent_false_with_attributes_with_different_parameters()
        {
            var csharpCode1 = @"
            [Foo(""Fred"", bar:3, bar2x:""Sam"")]
            public class Class1 {}
            ";
            var csharpCode2 = @"
            [Foo(""Fred"", bar2=""Sam"", bar=3)]
            public struct Struct1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var class1 = root1.Classes.First();
            var struct1 = root2.Structures.First();
            var attribute1 = class1.Attributes.Attributes.First();
            var attribute2 = struct1.Attributes.Attributes.First();
            Assert.IsFalse(attribute1.SameIntent(attribute2));

        }


        [TestMethod, TestCategory(SameIntentAtrributesCategory)]
        public void Same_intent_false_with_attributes_with_different_values()
        {
            var csharpCode1 = @"
            [Foo(""Fred"", bar:4, bar2:""Sam"")]
            public class Class1 {}
            ";
            var csharpCode2 = @"
            [Foo(""Fred"", bar2=""Sam"", bar=3)]
            public struct Struct1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var class1 = root1.Classes.First();
            var struct1 = root2.Structures.First();
            var attribute1 = class1.Attributes.Attributes.First();
            var attribute2 = struct1.Attributes.Attributes.First();
            Assert.IsFalse(attribute1.SameIntent(attribute2));

        }
        #endregion

        #region same intent method
        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_true_with_same_method_declaration_in_different_contexts()
        {
            // This test presents a fundamental question - should SameIntent include the 
            // context. I'm saying no, because there are use cases that go across files -
            // variation of find clone and code evaluation for template creation
            var csharpCode1 = @"
            public class Class1 
            {
                public void Foo(string bar, int bar2) {}
            ";
            var csharpCode2 = @"
            public struct Struct1 
            {
                public void Foo(string bar, int bar2) {}
            }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root2.Structures.First().Methods.First();
            Assert.IsTrue(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_name()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
                public void Bar(string bar, int bar2) {}
            }
            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst("public void Bar", "public void Bar2");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_return_type()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
                public void Bar(string bar, int bar2) {}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst("public void Bar", "public int Bar");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_attributes()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
               [Foo(""Fred"", bar:3, bar2:""George"")] public void Bar(string bar, int bar2) {}           
               [Foo(""Fred"", bar:3, bar2:""George"")] public void Bar(string bar, int bar2) {}          
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"bar2:""George""", @"bar2:""GeorgeX""");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_parameter_names()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
                public void Bar(string bar, int bar2) {}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"bar2", @"bar2X");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_scope()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
                public void Bar(string bar, int bar2) {}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"public void", @"internal void");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_abstract_modifier()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
                public void Bar(string bar, int bar2) {}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"public void Bar", @"public abstract void Bar");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method__static_modifier()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
                public void Bar(string bar, int bar2) {}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"public void Bar", @"public static void Bar");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_sealed_modifier()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
                public void Bar(string bar, int bar2) {}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"public void Bar", @"public sealed void Bar");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_override_modifiers()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
                public void Bar(string bar, int bar2) {}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"public void Bar", @"public override void Bar");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_modifiers()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
                public void Bar(string bar, int bar2) {}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"public void Bar", @"public virtual void Bar");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_if_extension()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
                public void Bar(string bar, int bar2) {}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"(string bar", @"(this string bar");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        [TestMethod, TestCategory(SameIntentMethodCategory)]
        public void Same_intent_false_with_different_method_type_parameters()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public void Bar<T>(string bar, int bar2) {}
                public void Bar<T>(string bar, int bar2) {}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"Bar<T>(", @"Bar<T2>(");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var method1 = root1.Classes.First().Methods.First();
            var method2 = root1.Classes.First().Methods.Last();
            Assert.IsTrue(method1.SameIntent(method2));
            method1 = root2.Classes.First().Methods.First();
            method2 = root2.Classes.First().Methods.Last();
            Assert.IsFalse(method1.SameIntent(method2));
        }

        #endregion

        #region same intent property
        [TestMethod, TestCategory(SameIntentPropertyCategory)]
        public void Same_intent_true_with_same_property_declaration_in_different_contexts()
        {
            // This test presents a fundamental question - should SameIntent include the 
            // context. I'm saying no, because there are use cases that go across files -
            // variation of find clone and code evaluation for template creation
            var csharpCode1 = @"
            public class Class1 
            {
                public string Foo{get; set;}
            ";
            var csharpCode2 = @"
            public struct Struct1 
            {
                public string Foo{get; set;}
            }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var property1 = root1.Classes.First().Properties.First();
            var property2 = root2.Structures.First().Properties.First();
            Assert.IsTrue(property1.SameIntent(property2));
        }

        [TestMethod, TestCategory(SameIntentPropertyCategory)]
        public void Same_intent_false_with_different_property_name()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public string Bar{get; set;}
                public string Bar{get; set;}
            }
            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst("public string Bar", "public string Bar2");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var property1 = root1.Classes.First().Properties.First();
            var property2 = root1.Classes.First().Properties.Last();
            Assert.IsTrue(property1.SameIntent(property2));
            property1 = root2.Classes.First().Properties.First();
            property2 = root2.Classes.First().Properties.Last();
            Assert.IsFalse(property1.SameIntent(property2));
        }

        [TestMethod, TestCategory(SameIntentPropertyCategory)]
        public void Same_intent_false_with_different_property_return_type()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public string Bar{get; set;}
                public string Bar{get; set;}
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst("public string Bar", "public int Bar");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var property1 = root1.Classes.First().Properties.First();
            var property2 = root1.Classes.First().Properties.Last();
            Assert.IsTrue(property1.SameIntent(property2));
            property1 = root2.Classes.First().Properties.First();
            property2 = root2.Classes.First().Properties.Last();
            Assert.IsFalse(property1.SameIntent(property2));
        }

        [TestMethod, TestCategory(SameIntentPropertyCategory)]
        public void Same_intent_false_with_different_property_attributes()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
               [Foo(""Fred"", bar:3, bar2:""George"")] public string Bar{get; set;}           
               [Foo(""Fred"", bar:3, bar2:""George"")] public string Bar{get; set;}          
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"bar2:""George""", @"bar2:""GeorgeX""");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var property1 = root1.Classes.First().Properties.First();
            var property2 = root1.Classes.First().Properties.Last();
            Assert.IsTrue(property1.SameIntent(property2));
            property1 = root2.Classes.First().Properties.First();
            property2 = root2.Classes.First().Properties.Last();
            Assert.IsFalse(property1.SameIntent(property2));
        }

        // I am not testing all of the modifiers becuase I think it's sufficient to test that once

        #endregion

        #region same intent field
        [TestMethod, TestCategory(SameIntentFieldCategory)]
        public void Same_intent_true_with_same_field_declaration_in_different_contexts()
        {
            // This test presents a fundamental question - should SameIntent include the 
            // context. I'm saying no, because there are use cases that go across files -
            // variation of find clone and code evaluation for template creation
            var csharpCode1 = @"
            public class Class1 
            {
                public string Foo;
            ";
            var csharpCode2 = @"
            public struct Struct1 
            {
                public string Foo;
            }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var field1 = root1.Classes.First().Fields.First();
            var field2 = root2.Structures.First().Fields.First();
            Assert.IsTrue(field1.SameIntent(field2));
        }

        [TestMethod, TestCategory(SameIntentFieldCategory)]
        public void Same_intent_false_with_different_field_name()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public string Bar;
                public string Bar;
            }
            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst("public string Bar", "public string Bar2");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var field1 = root1.Classes.First().Fields.First();
            var field2 = root1.Classes.First().Fields.Last();
            Assert.IsTrue(field1.SameIntent(field2));
            field1 = root2.Classes.First().Fields.First();
            field2 = root2.Classes.First().Fields.Last();
            Assert.IsFalse(field1.SameIntent(field2));
        }

        [TestMethod, TestCategory(SameIntentFieldCategory)]
        public void Same_intent_false_with_different_field_return_type()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
                public string Bar;
                public string Bar;
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst("public string Bar", "public int Bar");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var field1 = root1.Classes.First().Fields.First();
            var field2 = root1.Classes.First().Fields.Last();
            Assert.IsTrue(field1.SameIntent(field2));
            field1 = root2.Classes.First().Fields.First();
            field2 = root2.Classes.First().Fields.Last();
            Assert.IsFalse(field1.SameIntent(field2));
        }

        [TestMethod, TestCategory(SameIntentFieldCategory)]
        public void Same_intent_false_with_different_field_attributes()
        {
            var csharpCodeGood = @"
            public class Class1 
            {
               [Foo(""Fred"", bar:3, bar2:""George"")] public string Bar;           
               [Foo(""Fred"", bar:3, bar2:""George"")] public string Bar;         
            }

            ";
            var csharpCodeBad = csharpCodeGood.ReplaceFirst(@"bar2:""George""", @"bar2:""GeorgeX""");
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeGood);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeBad);
            var field1 = root1.Classes.First().Fields.First();
            var field2 = root1.Classes.First().Fields.Last();
            Assert.IsTrue(field1.SameIntent(field2));
            field1 = root2.Classes.First().Fields.First();
            field2 = root2.Classes.First().Fields.Last();
            Assert.IsFalse(field1.SameIntent(field2));
        }

        #endregion

        #region same intent class
        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_true_with_same_class_declaration_in_different_contexts()
        {
            // This test presents a fundamental question - should SameIntent include the 
            // context. I'm saying no, because there are use cases that go across files -
            // variation of find clone and code evaluation for template creation
            var csharpCode1 = @"
                namespace Namespace1
                {
                   namespace Namespace2
                   {
                      public class Class1 
                      {  public string Foo;}
                   }
                }
            ";
            var csharpCode2 = @"
                namespace Namespace1.Namespace2
                {
                    public class Class1 
                    {
                        public string Foo;
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_true_with_same_class_declaration_with_base_and_type_parameters()
        {
            // This test presents a fundamental question - should SameIntent include the 
            // context. I'm saying no, because there are use cases that go across files -
            // variation of find clone and code evaluation for template creation
            var csharpCode1 = @"
                namespace Namespace1
                {
                   namespace Namespace2
                   {
                    public class Class1<T1, T2> : baseClass
                    {   public string Foo;}
                    }
                }
            ";
            var csharpCode2 = @"
                namespace Namespace1.Namespace2
                {
                    public class Class1 <T1, T2> : baseClass
                    {
                        public string Foo;
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_false_with_different_class_name()
        {
            var csharpCode = @"
            public class Class1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("Class1", "Class2");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            class2 = root2.RootClasses.First();
            Assert.IsFalse(class1.SameIntent(class2));
        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_false_with_different_class_base_type()
        {
            var csharpCode = @"
            public class Class1 :classBase {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("classBase", "classBase2");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            class2 = root2.RootClasses.First();
            Assert.IsFalse(class1.SameIntent(class2));
        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_false_with_different_class_type_parameters()
        {
            var csharpCode = @"
            public class Class1<T1> {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("<T1>", "<T2>");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            class2 = root2.RootClasses.First();
            Assert.IsFalse(class1.SameIntent(class2));
        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_false_with_different_class_access_modifiers()
        {
            var csharpCode = @"
            public class Class1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("public", "protected internal");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            class2 = root2.RootClasses.First();
            Assert.IsFalse(class1.SameIntent(class2));
        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_false_with_different_class_static_modifiers()
        {
            var csharpCode = @"
                    public class Class1 {}
                    ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("public", "public static ");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            class2 = root2.RootClasses.First();
            Assert.IsFalse(class1.SameIntent(class2));

        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_false_with_different_class_sealed_modifiers()
        {
            var csharpCode = @"
                    public class Class1 {}
                    ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("public", "public sealed ");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            class2 = root2.RootClasses.First();
            Assert.IsFalse(class1.SameIntent(class2));

        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_false_with_different_class_abstract_modifiers()
        {
            var csharpCode = @"
            public class Class1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("public", "public abstract ");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            class2 = root2.RootClasses.Last();
            Assert.IsFalse(class1.SameIntent(class2));

        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_false_with_different_class_attributes()
        {
            var csharpCode = @"
            [Foo(var:""Sam"")]
            public class Class1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
            var csharpCodeChanged = csharpCode.ReplaceFirst(@"var:""Sam""", @"var:""George""");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            class2 = root2.RootClasses.Last();
            Assert.IsFalse(class1.SameIntent(class2));

        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_false_with_different_class_different_property()
        {
            var csharpCode = @"
            public class Class1 
            {
                public string Bar{ get;}
            }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("Bar", "Bar2");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            class2 = root2.RootClasses.Last();
            Assert.IsFalse(class1.SameIntent(class2));

        }

        [TestMethod, TestCategory(SameIntentClassCategory)]
        public void Same_intent_false_with_different_class_different_member_attribute()
        {
            var csharpCode = @"
            public class Class1 
            {
                [Foo()]
                public string Bar{ get;}
            }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var class1 = root1.RootClasses.First();
            var class2 = root2.RootClasses.First();
            Assert.IsTrue(class1.SameIntent(class2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("Foo", "Foo2");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            class2 = root2.RootClasses.Last();
            Assert.IsFalse(class1.SameIntent(class2));

        }

        #endregion

        #region same intent structure
        [TestMethod, TestCategory(SameIntentStructureCategory)]
        public void Same_intent_true_with_same_structure_declaration_in_different_contexts()
        {
            var csharpCode1 = @"
                namespace Namespace1
                {
                   namespace Namespace2
                   {
                      public struct Structure1 
                      {  public string Foo;}
                   }
                }
            ";
            var csharpCode2 = @"
                namespace Namespace1.Namespace2
                {
                    public struct Structure1 
                    {
                        public string Foo;
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var structure1 = root1.RootStructures.First();
            var structure2 = root2.RootStructures.First();
            Assert.IsTrue(structure1.SameIntent(structure2));
        }

        [TestMethod, TestCategory(SameIntentStructureCategory)]
        public void Same_intent_true_with_same_structure_declaration_with_type_parameters()
        {
            // This test presents a fundamental question - should SameIntent include the 
            // context. I'm saying no, because there are use cases that go across files -
            // variation of find clone and code evaluation for template creation
            var csharpCode1 = @"
                namespace Namespace1
                {
                   namespace Namespace2
                   {
                    public struct Structure1<T1, T2> 
                    {   public string Foo;}
                    }
                }
            ";
            var csharpCode2 = @"
                namespace Namespace1.Namespace2
                {
                    public struct Structure1<T1, T2> 
                    {
                        public string Foo;
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var structure1 = root1.RootStructures.First();
            var structure2 = root2.RootStructures.First();
            Assert.IsTrue(structure1.SameIntent(structure2));
        }

        [TestMethod, TestCategory(SameIntentStructureCategory)]
        public void Same_intent_false_with_different_structure_name()
        {
            var csharpCode = @"
            public struct Structure1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var structure1 = root1.RootStructures.First();
            var structure2 = root2.RootStructures.First();
            Assert.IsTrue(structure1.SameIntent(structure2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("Structure1", "Structure2");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            structure2 = root2.RootStructures.First();
            Assert.IsFalse(structure1.SameIntent(structure2));
        }

        [TestMethod, TestCategory(SameIntentStructureCategory)]
        public void Same_intent_false_with_different_structure_type_parameters()
        {
            var csharpCode = @"
            public struct Structure1<T1> {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var structure1 = root1.RootStructures.First();
            var structure2 = root2.RootStructures.First();
            Assert.IsTrue(structure1.SameIntent(structure2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("<T1>", "<T2>");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            structure2 = root2.RootStructures.First();
            Assert.IsFalse(structure1.SameIntent(structure2));

        }

        [TestMethod, TestCategory(SameIntentStructureCategory)]
        public void Same_intent_false_with_different_structure_access_modifiers()
        {
            var csharpCode = @"
            public struct Structure1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var structure1 = root1.RootStructures.First();
            var structure2 = root2.RootStructures.First();
            Assert.IsTrue(structure1.SameIntent(structure2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("public", "protected internal");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            structure2 = root2.RootStructures.First();
            Assert.IsFalse(structure1.SameIntent(structure2));
        }

        [TestMethod, TestCategory(SameIntentStructureCategory)]
        public void Same_intent_false_with_different_structure_attributes()
        {
            var csharpCode = @"
            [Foo(var:""Sam"")]
            public struct Structure1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var structure1 = root1.RootStructures.First();
            var structure2 = root2.RootStructures.First();
            Assert.IsTrue(structure1.SameIntent(structure2));
            var csharpCodeChanged = csharpCode.ReplaceFirst(@"var:""Sam""", @"var:""George""");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            structure2 = root2.RootStructures.First();
            Assert.IsFalse(structure1.SameIntent(structure2));
        }

        [TestMethod, TestCategory(SameIntentStructureCategory)]
        public void Same_intent_false_with_different_structure_member()
        {
            var csharpCode = @"
            public struct Structure1 
            {
                private int Foo;
            }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var structure1 = root1.RootStructures.First();
            var structure2 = root2.RootStructures.First();
            Assert.IsTrue(structure1.SameIntent(structure2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("Foo;", "Foo, Foo2;");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            structure2 = root2.RootStructures.First();
            Assert.IsFalse(structure1.SameIntent(structure2));
        }


        #endregion

        #region same intent interface
        [TestMethod, TestCategory(SameIntentInterfaceCategory)]
        public void Same_intent_true_with_same_interface_declaration_in_different_contexts()
        {
            // This test presents a fundamental question - should SameIntent include the 
            // context. I'm saying no, because there are use cases that go across files -
            // variation of find clone and code evaluation for template creation
            var csharpCode1 = @"
                namespace Namespace1
                {
                    namespace Namespace2
                    {
                        public interface Interface1
                        { string Foo{get;} }
                    }
                }
            ";
            var csharpCode2 = @"
                namespace Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        string Foo{get;}
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var interface1 = root1.RootInterfaces.First();
            var interface2 = root2.RootInterfaces.First();
            Assert.IsTrue(interface1.SameIntent(interface2));
        }

        [TestMethod, TestCategory(SameIntentInterfaceCategory)]
        public void Same_intent_true_with_same_interface_declaration_with_base_and_type_parameters()
        {
            // This test presents a fundamental question - should SameIntent include the 
            // context. I'm saying no, because there are use cases that go across files -
            // variation of find clone and code evaluation for template creation
            var csharpCode1 = @"
                namespace Namespace1
                {
                   namespace Namespace2
                   {
                    public interface Interface1<T1, T2> : baseInterface
                    {   public string Foo;}
                    }
                }
            ";
            var csharpCode2 = @"
                namespace Namespace1.Namespace2
                {
                    public interface Interface1 <T1, T2> : baseInterface
                    {
                        public string Foo;
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var interface1 = root1.RootInterfaces.First();
            var interface2 = root2.RootInterfaces.First();
            Assert.IsTrue(interface1.SameIntent(interface2));
        }

        [TestMethod, TestCategory(SameIntentInterfaceCategory)]
        public void Same_intent_false_with_different_interface_name()
        {
            var csharpCode = @"
            public interface Interface1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var interface1 = root1.RootInterfaces.First();
            var interface2 = root2.RootInterfaces.First();
            Assert.IsTrue(interface1.SameIntent(interface2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("Interface1", "Interface2");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            interface2 = root2.RootInterfaces.First();
            Assert.IsFalse(interface1.SameIntent(interface2));
        }

        [TestMethod, TestCategory(SameIntentInterfaceCategory)]
        public void Same_intent_false_with_different_interface_base_type()
        {
            var csharpCode = @"
            public interface Interface1  : baseInterface {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var interface1 = root1.RootInterfaces.First();
            var interface2 = root2.RootInterfaces.First();
            Assert.IsTrue(interface1.SameIntent(interface2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("baseInterface", "baseInterface2");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            interface2 = root2.RootInterfaces.First();
            Assert.IsFalse(interface1.SameIntent(interface2));
        }

        [TestMethod, TestCategory(SameIntentInterfaceCategory)]
        public void Same_intent_false_with_different_interface_type_parameters()
        {
            var csharpCode = @"
            public interface Interface1<T1, T2> {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var interface1 = root1.RootInterfaces.First();
            var interface2 = root2.RootInterfaces.First();
            Assert.IsTrue(interface1.SameIntent(interface2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("T2", "T3");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            interface2 = root2.RootInterfaces.First();
            Assert.IsFalse(interface1.SameIntent(interface2));
        }

        [TestMethod, TestCategory(SameIntentInterfaceCategory)]
        public void Same_intent_false_with_different_interface_access_modifiers()
        {
            var csharpCode = @"
            public interface Interface1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var interface1 = root1.RootInterfaces.First();
            var interface2 = root2.RootInterfaces.First();
            Assert.IsTrue(interface1.SameIntent(interface2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("public", "protected");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            interface2 = root2.RootInterfaces.First();
            Assert.IsFalse(interface1.SameIntent(interface2));
        }

        [TestMethod, TestCategory(SameIntentInterfaceCategory)]
        public void Same_intent_false_with_different_interface_attributes()
        {
            var csharpCode = @"
            [Foo(var:""Sam"")]
            public interface Interface1 {}
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var interface1 = root1.RootInterfaces.First();
            var interface2 = root2.RootInterfaces.First();
            Assert.IsTrue(interface1.SameIntent(interface2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("Foo", "Foo2");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            interface2 = root2.RootInterfaces.First();
            Assert.IsFalse(interface1.SameIntent(interface2));
        }

        [TestMethod, TestCategory(SameIntentInterfaceCategory)]
        public void Same_intent_false_with_different_interface_member()
        {
            var csharpCode = @"
            public interface Interface1 
            {
                void Foo();
            }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var interface1 = root1.RootInterfaces.First();
            var interface2 = root2.RootInterfaces.First();
            Assert.IsTrue(interface1.SameIntent(interface2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("void", "int");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            interface2 = root2.RootInterfaces.First();
            Assert.IsFalse(interface1.SameIntent(interface2));
        }

        #endregion

       #region same intent namespace

        [TestMethod, TestCategory(SameIntentNamespaceCategory)]
        public void Same_intent_false_with_same_namespace_declaration_in_different_contexts()
        {
            // THis test clarifies behavior. While context is not important in determining
            // same intent, the namespace itself differs here beuase it is named differently
            var csharpCode1 = @"
                namespace Namespace0
                {
                    namespace Namespace1
                    {
                        namespace Namespace2
                        {
                            public interface Interface1
                            { string Foo{get;} }
                        }
                    }
                 }
            ";
            var csharpCode2 = @"
                namespace Namespace0.Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        string Foo{get;}
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            var namespace1 = root1.NonemptyNamespaces.First();
            var namespace2 = root2.NonemptyNamespaces.First();
            Assert.IsFalse(namespace1.SameIntent(namespace2));
        }

        [TestMethod, TestCategory(SameIntentNamespaceCategory)]
        public void Same_intent_false_with_different_namespace_member()
        {
            var csharpCode = @"
                namespace Namespace0.Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        void Foo();
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var namespace1 = root1.NonemptyNamespaces .First();
            var namespace2 = root2.NonemptyNamespaces.First();
            Assert.IsTrue(namespace1.SameIntent(namespace2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("public", "private");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            namespace2 = root2.NonemptyNamespaces.First();
            Assert.IsFalse(namespace1.SameIntent(namespace2));
        }

        [TestMethod, TestCategory(SameIntentNamespaceCategory)]
        public void Same_intent_false_with_different_namespace_grandchild_member()
        {
            var csharpCode = @"
                namespace Namespace0.Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        void Foo();
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var namespace1 = root1.NonemptyNamespaces.First();
            var namespace2 = root2.NonemptyNamespaces.First();
            Assert.IsTrue(namespace1.SameIntent(namespace2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("void", "int");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            namespace2 = root2.NonemptyNamespaces.First();
            Assert.IsFalse(namespace1.SameIntent(namespace2));
        }


        #endregion

        #region same intent root

        [TestMethod, TestCategory(SameIntentRootCategory)]
        public void Same_intent_true_with_same_root_declaration_in_different_contexts()
        {
            // This test presents a fundamental question - should SameIntent include the 
            // context. I'm saying no, because there are use cases that go across files -
            // variation of find clone and code evaluation for template creation
            var csharpCode1 = @"
                namespace Namespace0
                {
                    namespace Namespace1
                    {
                        namespace Namespace2
                        {
                            public interface Interface1
                            { string Foo{get;} }
                        }
                    }
                 }
            ";
            var csharpCode2 = @"
                namespace Namespace0.Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        string Foo{get;}
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode1);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode2);
            Assert.IsTrue(root1.SameIntent(root2));
        }

        [TestMethod, TestCategory(SameIntentRootCategory)]
        public void Same_intent_false_with_different_root_member()
        {
            var csharpCode = @"
                namespace Namespace0.Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        void Foo();
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.IsTrue(root1.SameIntent(root2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("public", "private");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            Assert.IsFalse(root1.SameIntent(root2));
        }

        [TestMethod, TestCategory(SameIntentRootCategory)]
        public void Same_intent_false_with_different_root_grandchild_member()
        {
            var csharpCode = @"
                namespace Namespace0.Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        void Foo();
                    }
                }
            ";
            var root1 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.IsTrue(root1.SameIntent(root2));
            var csharpCodeChanged = csharpCode.ReplaceFirst("void", "int");
            root2 = RDomCSharpFactory.Factory.GetRootFromString(csharpCodeChanged);
            Assert.IsFalse(root1.SameIntent(root2));
        }


        #endregion


    }
}
