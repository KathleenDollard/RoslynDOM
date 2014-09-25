using System;
using System.Linq;
using ApprovalTests;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
      private const string SameIntentConstructorCategory = "SameIntentConstructor";
      private const string SameIntentDestructorCategory = "SameIntentDestructor";
      private const string SameIntentOperatorCategory = "SameIntentOperator";
      private const string SameIntentConversionOperatorCategory = "SameIntentConversionOperator";
      private const string SameIntentEventCategory = "SameIntentEvent";
      private const string SameIntentIndexerCategory = "SameIntentIndexer";
      private const string SameIntentClassCategory = "SameIntentClass";
      private const string SameIntentStructureCategory = "SameIntentStructure";
      private const string SameIntentInterfaceCategory = "SameIntentInterface";
      private const string SameIntentEnumCategory = "SameIntentEnum";
      private const string SameIntentNamespaceCategory = "SameIntentNamespace";
      private const string SameIntentUsingsCategory = "SameIntentUsings";
      private const string SameIntentRootCategory = "SameIntentRoot";
      private const string SameIntentStatementCategory = "SameIntentStatements";
      private const string SameIntentExpressionCategory = "SameIntentExpressions";
      private const string SameIntentSpecialCategory = "SameIntentSpecial";

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
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var attribute1 = root1.Classes.First().Attributes.Attributes.First();
         var attribute2 = root2.Structures.First().Attributes.Attributes.First();
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
            public class Class1 {}
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var class1 = root1.Classes.First();
         var class2 = root2.Classes.First();
         var attribute1 = class1.Attributes.Attributes.First();
         var attribute2 = class2.Attributes.Attributes.First();
         Assert.IsTrue(attribute1.SameIntent(attribute2));

      }

      [TestMethod, TestCategory(SameIntentAtrributesCategory)]
      public void Same_intent_true_with_attributes_stated_with_different_whitespace()
      {
         var csharpCode1 = @"
            [Foo(""Fred"", bar:3, bar2:""Sam"")]
            public class Class1 {}
            ";
         var csharpCode2 = @"
            [Foo(""Fred"", bar:3, 
                  bar2:""Sam"")]
            public class Class1 {}
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var class1 = root1.Classes.First();
         var class2 = root2.Classes.First();
         var attribute1 = class1.Attributes.Attributes.First();
         var attribute2 = class2.Attributes.Attributes.First();
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
            public class Class1 {}
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var class1 = root1.Classes.First();
         var class2 = root2.Classes.First();
         var attribute1 = class1.Attributes.Attributes.First();
         var attribute2 = class2.Attributes.Attributes.First();
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
            public class Class1 {}
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var class1 = root1.Classes.First();
         var class2 = root2.Classes.First();
         var attribute1 = class1.Attributes.Attributes.First();
         var attribute2 = class2.Attributes.Attributes.First();
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
            public class Class1 {}
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var class1 = root1.Classes.First();
         var class2 = root2.Classes.First();
         var attribute1 = class1.Attributes.Attributes.First();
         var attribute2 = class2.Attributes.Attributes.First();
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
            public class Class1 
            {
                public void Foo(string bar, int bar2) {}
            }
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var method1 = root1.Classes.First().Methods.First();
         var method2 = root2.Classes.First().Methods.First();
         Assert.IsTrue(method1.SameIntent(method2));
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_name()
      {
         var csharpCode =
         @"public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public void Bar", "public void Bar2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_return_type()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public void Bar", "public int Bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_attributes()
      {
         var csharpCode = @"
            public class Class1 
            {
               [Foo(""Fred"", bar:3, bar2:""George"")] public void Bar(string bar, int bar2) {}           
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"bar2:""George""", @"bar2:""GeorgeX""");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_parameter_names()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"bar2", @"bar2X");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_parameter_order()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"string bar, int bar2", @"int bar2, string bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_scope()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"public void", @"internal void");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_abstract_modifier()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"public void Bar", @"public abstract void Bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method__static_modifier()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"public void Bar", @"public static void Bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_sealed_modifier()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"public void Bar", @"public sealed void Bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_override_modifiers()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"public void Bar", @"public override void Bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_modifiers()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"public void Bar", @"public virtual void Bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_extension_this()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"(string bar", @"(this string bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_type_parameters()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar<T>(string bar, int bar2) {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"Bar<T>(", @"Bar<T2>(");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_type_parameter_constraints()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar<T>(string bar, int bar2) where T : class, IDom
                {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@", IDom", @", IDom, IHasName");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentMethodCategory)]
      public void Same_intent_false_with_different_method_type_parameter_one_has_no_constraints()
      {
         var csharpCode = @"
            public class Class1 
            {
                public void Bar<T>(string bar, int bar2)
                {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@", int bar2)", @", int bar2) where T : value");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }


      #endregion

      #region same intent property
      [TestMethod, TestCategory(SameIntentPropertyCategory)]
      public void Same_intent_true_with_same_property_declaration_in_different_contexts()
      {
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
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var property1 = root1.Classes.First().Properties.First();
         var property2 = root2.Structures.First().Properties.First();
         Assert.IsTrue(property1.SameIntent(property2));
      }

      [TestMethod, TestCategory(SameIntentPropertyCategory)]
      public void Same_intent_false_with_different_property_name()
      {
         var csharpCode = @"
            public class Class1 
            {
                public string Bar{get; set;}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public string Bar", "public string Bar2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentPropertyCategory)]
      public void Same_intent_false_with_different_property_return_type()
      {
         var csharpCode = @"
            public class Class1 
            {
                public string Bar{get; set;}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public string Bar", "public int Bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentPropertyCategory)]
      public void Same_intent_false_with_different_property_generic_return_type()
      {
         var csharpCode = @"
            public class Class1 
            {
                public IEnumerable<string> Bar{get; set;}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("string", "int");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentPropertyCategory)]
      public void Same_intent_false_with_different_property_generic_return_type_with_get_body()
      {
         var csharpCode = @"
            public class Class1 
            {
               public IEnumerable<string> Bar
               {get
                     { return null;}
               }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("string", "int");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentPropertyCategory)]
      public void Same_intent_false_with_different_property_attributes()
      {
         var csharpCode = @"
            public class Class1 
            {
               [Foo(""Fred"", bar:3, bar2:""George"")] public string Bar{get; set;}           
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"bar2:""George""", @"bar2:""GeorgeX""");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      // I am not testing all of the modifiers becuase I think it's sufficient to test that once

      [TestMethod, TestCategory(SameIntentPropertyCategory)]
      public void Same_intent_false_with_different_property_write_only()
      {
         var csharpCode = @"
            public class Class1 
            {
               [Foo(""Fred"", bar:3, bar2:""George"")] public string Bar{get; set;}           
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("get;", "");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentPropertyCategory)]
      public void Same_intent_false_with_different_property_read_only()
      {
         var csharpCode = @"
            public class Class1 
            {
               [Foo(""Fred"", bar:3, bar2:""George"")] public string Bar{get; set;}           
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("set;", "");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentPropertyCategory)]
      public void Same_intent_false_with_different_property_is_static()
      {
         var csharpCode = @"
            public class Class1 
            {
               public string Bar{get; set;}           
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public string ", "public static string ");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentPropertyCategory)]
      public void Same_intent_false_with_different_property_is_new()
      {
         var csharpCode = @"
            public class Class1 
            {
               public string Bar{get; set;}           
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public string", "public new string");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }
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
            { public string Foo; }
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var field1 = root1.Classes.First().Fields.First();
         var field2 = root2.Structures.First().Fields.First();
         Assert.IsTrue(field1.SameIntent(field2));
      }

      [TestMethod, TestCategory(SameIntentFieldCategory)]
      public void Same_intent_false_with_with_different_field_is_new()
      {
         var csharpCode = @"
            public class Class1 
            {
                public string Foo;
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public string Foo;", "public new string Foo;");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentFieldCategory)]
      public void Same_intent_false_with_with_different_field_is_const()
      {
         var csharpCode = @"
            public class Class1 
            {
                public static string Foo = ""Fred"";
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("static", "const");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentFieldCategory)]
      public void Same_intent_false_with_with_different_field_is_volatile()
      {
         var csharpCode = @"
            public class Class1 
            {
                public string Foo;
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public string Foo;", "public volatile string Foo;");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentFieldCategory)]
      public void Same_intent_false_with_with_different_field_is_readonly()
      {
         var csharpCode = @"
            public class Class1 
            {
                public string Foo;
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public string Foo;", "public readonly string Foo;");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentFieldCategory)]
      public void Same_intent_false_with_different_field_name()
      {
         var csharpCode = @"
            public class Class1 
            {
                public string Bar;
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public string Bar", "public string Bar2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentFieldCategory)]
      public void Same_intent_false_with_different_field_return_type()
      {
         var csharpCode = @"
            public class Class1 
            {
                public string Bar;
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public string Bar", "public int Bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentFieldCategory)]
      public void Same_intent_false_with_different_field_static()
      {
         var csharpCode = @"
            public class Class1 
            {
                public string Bar;
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public string Bar", "public static string Bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentFieldCategory)]
      public void Same_intent_false_with_different_field_attributes()
      {
         var csharpCode = @"
            public class Class1 
            {
               [Foo(""Fred"", bar:3, bar2:""George"")] public string Bar;           
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"bar2:""George""", @"bar2:""GeorgeX""");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }


      [TestMethod, TestCategory(SameIntentFieldCategory)]
      public void Same_intent_false_with_different_field_initializers()
      {
         var csharpCode = @"
            public class Class1 
            {
               public string Bar = ""Fred"";           
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"Fred", @"Fred2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentFieldCategory)]
      public void Same_intent_false_with_removed_field_initializers()
      {
         var csharpCode = @"
            public class Class1 
            {
               public string Bar = ""Fred"";           
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"Bar = ""Fred""", @"Bar");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      #endregion

      #region same intent constructor
      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_true_with_same_constructor_declaration_in_different_contexts()
      {
         // It may seem odd that this test passes. The class name is not currently 
         // part of the constructor and this is consistent with VB. 
         var csharpCode1 = @"
            public class Class1 
            {
                public Class1(){}
            }";
         var csharpCode2 = @"
            public struct Struct1 
            { public Struct1(){} }
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var constr1 = root1.Classes.First().Constructors.First();
         var constr2 = root2.Structures.First().Constructors.First();
         Assert.IsTrue(constr1.SameIntent(constr2));
      }

      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_false_with_with_different_constructor_accessiblity()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1() {}
            }";
         var csharpCodeChanged = csharpCode.Replace("public Class1()", "internal Class1()");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_false_with_with_different_constructor_parameter_name()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x) {}
            }";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode);
         Assert.AreEqual(1, root1.Classes.First().Constructors.Count());

         var csharpCodeChanged = csharpCode.Replace("x", "y");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      public void Same_intent_false_with_with_different_constructor_parameter_type()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x) {};
            }";
         var csharpCodeChanged = csharpCode.Replace("int", "string");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      public void Same_intent_false_with_with_different_constructor_parameter_count()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x) {}
            }";
         var csharpCodeChanged = csharpCode.Replace("x", "x, int y");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_false_with_with_different_constructor_initializer_removed_base()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x) : base()
                {}
            }";
         var csharpCodeChanged = csharpCode.Replace(": base()", "");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_false_with_with_different_constructor_base_arguments()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x, int y) : base(x, y)
                {}
            }";
         var csharpCodeChanged = csharpCode.Replace("x, y", "x");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_false_with_with_different_constructor_initializer_base_to_this()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x) : base()
                {}
            } ";
         var csharpCodeChanged = csharpCode.Replace("base()", "this()");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_false_with_with_different_constructor_initializer_add_parameter()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x) : base()
                {}
            }";
         var csharpCodeChanged = csharpCode.Replace("base()", "base(x)");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_false_with_with_different_constructor_is_static()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x) : base()
                {}
            }";
         var csharpCodeChanged = csharpCode.Replace("public Class1(", "public static Class1(");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_false_with_with_different_constructor_statements_removed()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x) : base()
                {
                    this.X = x;
                }
            }";
         var csharpCodeChanged = csharpCode.Replace("this.X = x;\r\n", "");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_false_with_with_different_constructor_statements_altered()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x) : base()
                {
                    this.X = x;
                }
            }";
         var csharpCodeChanged = csharpCode.Replace("this.X = x;", "this.Y = x;");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentConstructorCategory)]
      public void Same_intent_false_with_with_different_constructor_base_to_this()
      {
         var csharpCode = @"
            public class Class1 
            {
                public Class1(int x) : base()
                {}
            }";
         var csharpCodeChanged = csharpCode.Replace("base()", "this()");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }
      #endregion

      #region same intent destructor
      [TestMethod, TestCategory(SameIntentDestructorCategory)]
      public void Same_intent_true_with_same_destructor_declaration_in_different_contexts()
      {
         Assert.Inconclusive();
      }

      [TestMethod, TestCategory(SameIntentDestructorCategory)]
      public void Same_intent_false_with_with_different_destructor_accessiblity()
      {
         Assert.Inconclusive();
      }
      #endregion

      #region same intent operator
      [TestMethod, TestCategory(SameIntentOperatorCategory)]
      public void Same_intent_true_with_same_operator_declaration_in_different_contexts()
      {
         Assert.Inconclusive();
      }

      [TestMethod, TestCategory(SameIntentOperatorCategory)]
      public void Same_intent_false_with_with_different_operator_accessiblity()
      {
         Assert.Inconclusive();
      }
      #endregion

      #region same intent conversion_operator
      [TestMethod, TestCategory(SameIntentConversionOperatorCategory)]
      public void Same_intent_true_with_same_conversion_operator_declaration_in_different_contexts()
      {
         Assert.Inconclusive();
      }

      [TestMethod, TestCategory(SameIntentConversionOperatorCategory)]
      public void Same_intent_false_with_with_different_conversion_operator_accessiblity()
      {
         Assert.Inconclusive();
      }
      #endregion

      #region same intent event
      [TestMethod, TestCategory(SameIntentEventCategory)]
      public void Same_intent_true_with_same_event_declaration_in_different_contexts()
      {
         Assert.Inconclusive();
      }

      [TestMethod, TestCategory(SameIntentEventCategory)]
      public void Same_intent_false_with_with_different_event_accessiblity()
      {
         Assert.Inconclusive();
      }
      #endregion

      #region same intent indexer
      [TestMethod, TestCategory(SameIntentIndexerCategory)]
      public void Same_intent_true_with_same_indexer_declaration_in_different_contexts()
      {
         Assert.Inconclusive();
      }

      [TestMethod, TestCategory(SameIntentIndexerCategory)]
      public void Same_intent_false_with_with_different_indexer_accessiblity()
      {
         Assert.Inconclusive();
      }
      #endregion


      #region same intent class
      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_true_with_same_class_declaration_in_different_contexts()
      {
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
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var class1 = root1.RootClasses.First();
         var class2 = root2.RootClasses.First();
         Assert.IsTrue(class1.SameIntent(class2));
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_true_with_same_class_declaration_with_base_and_type_parameters()
      {
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
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var class1 = root1.RootClasses.First();
         var class2 = root2.RootClasses.First();
         Assert.IsTrue(class1.SameIntent(class2));
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_name()
      {
         var csharpCode = @"
            public class Class1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Class1", "Class2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_base_type()
      {
         var csharpCode = @"
            public class Class1 :classBase {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("classBase", "classBase2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_type_parameters()
      {
         var csharpCode = @"
            public class Class1<T1> {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("<T1>", "<T2>");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_type_parameters_with_constraints()
      {
         var csharpCode = @"
            public class Class1 < T1 >
                where T1 : Foo
            {}";
         var csharpCodeChanged = csharpCode.Replace("T1", "T2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_type_parameters_with_many_constraints()
      {
         var csharpCode = @"
            public class Class1<T1>
                where T1 : class,Foo<T1>, IFoo, new()
            {}";
         var csharpCodeChanged = csharpCode.Replace("T1", "T2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_access_modifiers()
      {
         var csharpCode = @"
            public class Class1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public", "protected internal");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_static_modifiers()
      {
         var csharpCode = @"
                    public class Class1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public", "public static ");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_sealed_modifiers()
      {
         var csharpCode = @"
                    public class Class1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public", "public sealed ");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_abstract_modifiers()
      {
         var csharpCode = @"
            public class Class1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public", "public abstract ");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_attributes()
      {
         var csharpCode = @"
            [Foo(var:""Sam"")]
            public class Class1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"var:""Sam""", @"var:""George""");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_different_property()
      {
         var csharpCode = @"
            public class Class1 
            {
                public string Bar{ get;}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Bar", "Bar2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_class_different_member_attribute()
      {
         var csharpCode = @"
            public class Class1 
            {
                [Foo]
                public string Bar{ get;}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Foo", "Foo2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_nested_class()
      {
         var csharpCode = @"
            public class Class1 
            {
                public class Class2 {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Class2", "Class3");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_implemented_interfaces_on_class()
      {
         var csharpCode = @"
            public class Class1 : ClassB, IWhatever
            {  }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("IWhatever", "IWhatever2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_nested_interfaces()
      {
         var csharpCode = @"
            public class Class1 
            { 
                public interface Interface1{ }
            }           ";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Interface1", "Interface2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentClassCategory)]
      public void Same_intent_false_with_different_nested_enums()
      {
         var csharpCode = @"
            public class Class1 
            { 
                public enum Enum1{ red, yellow, blue }
            }           ";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Enum1", "Enum2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
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
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var structure1 = root1.RootStructures.First();
         var structure2 = root2.RootStructures.First();
         Assert.IsTrue(structure1.SameIntent(structure2));
      }

      [TestMethod, TestCategory(SameIntentStructureCategory)]
      public void Same_intent_true_with_same_structure_declaration_with_type_parameters()
      {
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
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var structure1 = root1.RootStructures.First();
         var structure2 = root2.RootStructures.First();
         Assert.IsTrue(structure1.SameIntent(structure2));
      }

      [TestMethod, TestCategory(SameIntentStructureCategory)]
      public void Same_intent_false_with_different_structure_name()
      {
         var csharpCode = @"
            public struct Structure1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Structure1", "Structure2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStructureCategory)]
      public void Same_intent_false_with_different_structure_type_parameters()
      {
         var csharpCode = @"
            public struct Structure1<T1> {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("<T1>", "<T2>");
      }

      [TestMethod, TestCategory(SameIntentStructureCategory)]
      public void Same_intent_false_with_different_structure_access_modifiers()
      {
         var csharpCode = @"
            public struct Structure1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public", "protected internal");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStructureCategory)]
      public void Same_intent_false_with_different_structure_attributes()
      {
         var csharpCode = @"
            [Foo(var:""Sam"")]
            public struct Structure1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst(@"var:""Sam""", @"var:""George""");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStructureCategory)]
      public void Same_intent_false_with_different_structure_member()
      {
         var csharpCode = @"
            public struct Structure1 
            {
                private int Foo;
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Foo;", "Foo, Foo2;");
         // TODO: Check namespace after grouping them - remove true
         AssertSameIntent(false, csharpCode, csharpCodeChanged, true);
      }


      [TestMethod, TestCategory(SameIntentStructureCategory)]
      public void Same_intent_false_with_different_nested_structure()
      {
         var csharpCode = @"
            public struct Structure1 
            {
                public struct Structure2 {}
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Structure2", "Structure3");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStructureCategory)]
      public void Same_intent_false_with_different_implemented_interfaces_on_structure()
      {
         var csharpCode = @"
            public struct Structure1 : IWhatever
            {  }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("IWhatever", "IWhatever2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }
      #endregion

      #region same intent interface
      [TestMethod, TestCategory(SameIntentInterfaceCategory)]
      public void Same_intent_true_with_same_interface_declaration_in_different_contexts()
      {
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
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var interface1 = root1.RootInterfaces.First();
         var interface2 = root2.RootInterfaces.First();
         Assert.IsTrue(interface1.SameIntent(interface2));
      }

      [TestMethod, TestCategory(SameIntentInterfaceCategory)]
      public void Same_intent_true_with_same_interface_declaration_with_base_and_type_parameters()
      {
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
                        string Foo;
                    }
                }
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var interface1 = root1.RootInterfaces.First();
         var interface2 = root2.RootInterfaces.First();
         Assert.IsTrue(interface1.SameIntent(interface2));
      }

      [TestMethod, TestCategory(SameIntentInterfaceCategory)]
      public void Same_intent_false_with_different_interface_name()
      {
         var csharpCode = @"
            public interface Interface1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Interface1", "Interface2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentInterfaceCategory)]
      public void Same_intent_false_with_different_interface_base_type()
      {
         var csharpCode = @"
            public interface Interface1  : baseInterface {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("baseInterface", "baseInterface2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentInterfaceCategory)]
      public void Same_intent_false_with_different_interface_type_parameters()
      {
         var csharpCode = @"
            public interface Interface1<T1, T2> {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("T2", "T3");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentInterfaceCategory)]
      public void Same_intent_false_with_different_interface_access_modifiers()
      {
         var csharpCode = @"
            public interface Interface1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public", "protected");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentInterfaceCategory)]
      public void Same_intent_false_with_different_interface_attributes()
      {
         var csharpCode = @"
            [Foo(var:""Sam"")]
            public interface Interface1 {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Foo", "Foo2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentInterfaceCategory)]
      public void Same_intent_false_with_different_interface_member()
      {
         var csharpCode = @"
            public interface Interface1 
            {
                void Foo();
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("void", "int");
      }


      [TestMethod, TestCategory(SameIntentInterfaceCategory)]
      public void Same_intent_false_with_different_implemented_interfaces_on_interface()
      {
         var csharpCode = @"
            public interface Interface1 : IWhatever
            {  }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("IWhatever", "IWhatever2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }
      #endregion


      #region same intent enum
      [TestMethod, TestCategory(SameIntentEnumCategory)]
      public void Same_intent_true_with_same_enum_declaration_in_different_contexts()
      {
         var csharpCode1 = @"
                namespace Namespace1
                {
                    namespace Namespace2
                    {
                        public enum Colors
                        { Red, Yellow, Green }
                    }
                }
            ";
         var csharpCode2 = @"
                namespace Namespace1.Namespace2
                {
                     public enum Colors
                     {  Red, 
                        Yellow,
                        Green 
                     }
                }
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var enum1 = root1.RootEnums.First();
         var enum2 = root2.RootEnums.First();
         Assert.IsTrue(enum1.SameIntent(enum2));
      }

      [TestMethod, TestCategory(SameIntentEnumCategory)]
      public void Same_intent_true_with_same_enum_declaration_with_base_and_type_parameters()
      {
         var csharpCode1 = @"
                namespace Namespace1
                {
                    namespace Namespace2
                    {
                        public enum Colors : long
                        { Red, Yellow, Green }
                    }
                }
            ";
         var csharpCode2 = @"
                namespace Namespace1.Namespace2
                {
                     public enum Colors : long
                     {  Red, 
                        Yellow,
                        Green 
                     }
                }
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var enum1 = root1.RootEnums.First();
         var enum2 = root2.RootEnums.First();
         Assert.IsTrue(enum1.SameIntent(enum2));
      }

      [TestMethod, TestCategory(SameIntentEnumCategory)]
      public void Same_intent_false_with_different_enum_name()
      {
         var csharpCode = @"
            public enum Colors {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Colors", "Colors2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentEnumCategory)]
      public void Same_intent_false_with_different_enum_underlying_type()
      {
         var csharpCode = @"
            public enum Colors : int
            {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("int", "long");
      }

      [TestMethod, TestCategory(SameIntentEnumCategory)]
      public void Same_intent_false_with_different_enum_attribute()
      {
         var csharpCode = @"
            [Fred]
            public enum Colors : int
            {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Fred", "George");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentEnumCategory)]
      public void Same_intent_false_with_different_enum_accessibliity()
      {
         var csharpCode = @"
            [Fred]
            public enum Colors : int
            {}";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public", "internal");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentEnumCategory)]
      public void Same_intent_false_with_different_enum_members()
      {
         var csharpCode = @"
            [Fred]
            public enum Colors : int
            {
                Red,
                Yellow,
                Blue
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Yellow", "Yellow2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentEnumCategory)]
      public void Same_intent_false_with_different_enum_stated_values()
      {
         var csharpCode = @"
            [Fred]
            public enum Colors : int
            {
                Red = 1,
                Yellow = 2,
                Blue
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("2", "3");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }
      #endregion

      #region same intent namespace

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
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public", "private");
         // TODO: Check namespace after grouping them - remove true
         AssertSameIntent(false, csharpCode, csharpCodeChanged, true);
      }

      [TestMethod, TestCategory(SameIntentNamespaceCategory)]
      public void Same_intent_false_with_different_namespace_name()
      {
         var csharpCode = @"
                namespace Namespace0.Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        void Foo();
                    }
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Namespace0", "NamespaceA");
         // TODO: Check namespace after grouping them - remove true
         AssertSameIntent(false, csharpCode, csharpCodeChanged, true);
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
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("void", "int");
         // TODO: Check namespace after grouping them - remove true
         AssertSameIntent(false, csharpCode, csharpCodeChanged, true);
      }
      #endregion

      #region same intent root

      [TestMethod, TestCategory(SameIntentRootCategory)]
      public void Same_intent_true_with_same_root_declaration_in_different_contexts()
      {
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
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
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
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("public", "private");
         // TODO: Check namespace after grouping them - remove true
         AssertSameIntent(false, csharpCode, csharpCodeChanged, true);
      }

      [TestMethod, TestCategory(SameIntentRootCategory)]
      public void Same_intent_false_with_different_root_namespace()
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
         var csharpCodeChanged = csharpCode.ReplaceFirst("Namespace0", "NamespaceA");
         // TODO: Check namespace after grouping them - remove true
         AssertSameIntent(false, csharpCode, csharpCodeChanged, true);
      }

      [TestMethod, TestCategory(SameIntentRootCategory)]
      public void Same_intent_false_with_different_root_name()
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
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode);
         Assert.IsTrue(root1.SameIntent(root2));
         root2.Name = "Fred";
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
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("void", "int");
         // TODO: Check namespace after grouping them - remove true
         AssertSameIntent(false, csharpCode, csharpCodeChanged, true);
      }

      [TestMethod, TestCategory(SameIntentRootCategory)]
      public void Same_intent_false_with_different_usings()
      {
         var csharpCode = @"
                using Fred.Flinstone;
                namespace Namespace0.Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        void Foo();
                    }
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("Fred", "Wilma");
         // TODO: Check namespace after grouping them - remove true
         AssertSameIntent(false, csharpCode, csharpCodeChanged, true);
      }
      #endregion

      #region same intent usings

      [TestMethod, TestCategory(SameIntentUsingsCategory)]
      public void Same_intent_true_with_same_using_in_different_contexts()
      {
         var csharpCode1 = @"
                
                using System;
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
                    using System;
                    public interface Interface1 
                    {
                        string Foo{get;}
                    }
                }
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var nspace2 = root2.Namespaces.First();
         Assert.IsTrue(root1.UsingDirectives.First().SameIntent(nspace2.UsingDirectives.First()));
      }

      [TestMethod, TestCategory(SameIntentRootCategory)]
      public void Same_intent_false_with_different_using()
      {
         var csharpCode = @"
                using System;
                using System.Collections;
                namespace Namespace0.Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        void Foo();
                    }
                }";
         // TODO: Check namespace after grouping them - remove true
         var csharpCodeChanged = csharpCode.ReplaceFirst("Collections", "Collections.Generic");
         AssertSameIntent(false, csharpCode, csharpCodeChanged, true);
      }

      [TestMethod, TestCategory(SameIntentRootCategory)]
      public void Same_intent_false_with_different_aliases()
      {
         var csharpCode = @"
                using A=System;
                using B=System.Collections;
                namespace Namespace0.Namespace1.Namespace2
                {
                    public interface Interface1 
                    {
                        void Foo();
                    }
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("B", "B2");
         // TODO: Check namespace after grouping them - remove true
         AssertSameIntent(false, csharpCode, csharpCodeChanged, true);
      }

      #endregion

      #region same intent statements

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_true_with_same_statements_in_different_contexts()
      {
         var csharpCode1 = @"
                 namespace Namespace2
                {
                    public class Class1
                    { 
                        string Foo()
                        {
                            var x = y + 42;
                        } 
                    }
                }
            ";
         var csharpCode2 = @"
              public struct Structure1
                    { 
                        string Foo
                        { get{
                            var x = y + 42;
                            }
                        } 
                    }
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var statement1 = root1.Descendants.OfType<IDeclarationStatement>().First();
         var statement2 = root2.Descendants.OfType<IDeclarationStatement>().First();
         Assert.IsTrue(statement1.SameIntent(statement2));
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_different_statements()
      {
         var csharpCode = @"
               public class ClassA 
                {
                    public void Foo()
                    { x = y + 2; }
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("x = y + 2;", "Console.WriteLine();");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_assignment_statement_expression()
      {
         var csharpCode = @"
                public class ClassA 
                {
                    public void Foo()
                    { x = y + 2; }
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("y + 2", "z + 2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_assignment_statement_variable()
      {
         var csharpCode = @"
                public class ClassA 
                {
                    public void Foo()
                    { x = y + 2; }
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("x", "z");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);

      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_assignment_statement_operator()
      {
         var csharpCode = @"
                public class ClassA 
                {
                    public void Foo()
                    { x = y + 2; }
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("=", "+=");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_block_statements()
      {
         var csharpCode = @"
                public class ClassA 
                {
                    public void Foo()
                    { 
                        {
                        x = y + 2;
                        Console.WriteLine();
                        var x = 42;
                        }
                    }
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("x = y + 3;", "XXX;")
                     .ReplaceFirst("Console.WriteLine();", "x = y + 2;")
                     .ReplaceFirst("XXX;", "Console.WriteLine();");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_checked_statements()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        checked
                        {
                        x = y + 2;
                        Console.WriteLine();
                        var x = 42;
                        }
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("checked", "unchecked");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_foreach_statements()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        foreach(var loopVar in longList)
                        {
                        x = y + 2;
                        Console.WriteLine();
                        var x = 42;
                        }
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("loopVar", "loopVar2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_for_statements_initializer()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        for (int i = 0; i < length; i++)
                        {
                        x = y + 2;
                        Console.WriteLine();
                        var x = 42;
                        }
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("0", "1");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_for_statement_iterator()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        for (int i = 0; i < length; i++)
                        {
                        x = y + 2;
                        Console.WriteLine();
                        var x = 42;
                        }
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("i++", "i--");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }


      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_if_statement_statement()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                    if (x == 1) { return a; }
                    else if (x == 1) { return b; }
                    else if (x == 2) { return c; }
                    else  { return d; }
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(" b", " b2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }
      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_if_condition()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                    if (x == 1) { return a; }
                    else if (x == 1) { return b; }
                    else if (x == 2) { return c; }
                    else  { return d; }
                    }
            }";

         var csharpCodeChanged = csharpCode.ReplaceFirst("x == 1", "x == 11");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }
      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_else_statmemnt()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                    if (x == 1) { return a; }
                    else if (x == 1) { return b; }
                    else if (x == 2) { return c; }
                    else  { return d; }
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("d", "d2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_else_condition()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                    if (x == 1) { return a; }
                    else if (x == 1) { return b; }
                    else if (x == 2) { return c; }
                    else  { return d; }
                    }
            }";

         var csharpCodeChanged = csharpCode.ReplaceFirst("x == 2", "x == 12");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }
      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_invocation_statements()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        Console.Writeline(42);
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("42", "43");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_lock_statements()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        lock (thisLock)
                        {
                            x = y + 2;
                            Console.WriteLine();
                            var x = 42;
                        }
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("thisLock", "thatLock");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_object_creation_expression_parameter()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        z = new Bar(x, y);
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("x", "x2");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_object_creation_expression_parameter_order()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        z = new Bar(x, y);
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("x, y", "y, x");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_return_statements()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public int Foo()
                   { 
                        return x;
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("x", "42");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_throw_statements_exception()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        throw new InvalidOperationException();
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("InvalidOperationException", "NotImplementedException");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_throw_statement_exception_parameter()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        throw new InvalidOperationException();
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("tion()", "tion(42)");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_try_statements()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        try
                        {
                            var x = 32;
                        }
                        catch (InvalidOperationException ex1)
                        { Console.WriteLine(ex1); }
                        catch (NotImplementedException)
                        { Console.WriteLine(ex2); }
                        finally { Console.WriteLine(42);}
                    }
            }";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var root2 = root1.Copy();
         var statement1 = root1.Classes.First().Methods.First().Statements.First() as ITryStatement;

         var csharpCodeChanged = csharpCode.ReplaceFirst("32", "320");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_try_finally_statement()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        try
                        {
                            var x = 32;
                        }
                        catch (InvalidOperationException ex1)
                        { Console.WriteLine(ex1); }
                        catch (NotImplementedException)
                        { Console.WriteLine(ex2); }
                        finally { Console.WriteLine(42);}
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("42", "420");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_try_finally_removed()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        try
                        {
                            var x = 32;
                        }
                        catch (InvalidOperationException ex1)
                        { Console.WriteLine(ex1); }
                        catch (NotImplementedException)
                        { Console.WriteLine(ex2); }
                        finally { Console.WriteLine(42);}
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("finally", "// finally");
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_try_catch_variable_name()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        try
                        {
                            var x = 32;
                        }
                        catch (InvalidOperationException ex1)
                        { Console.WriteLine(ex1); }
                        catch (NotImplementedException)
                        { Console.WriteLine(ex2); }
                        finally { Console.WriteLine(42);}
                    }
            }";

         var csharpCodeChanged = csharpCode.ReplaceFirst("ex1", "ex1B");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_try_catch_added()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        try
                        {
                            var x = 32;
                        }
                        catch (InvalidOperationException ex1)
                        { Console.WriteLine(ex1); }
                        catch (NotImplementedException)
                        { Console.WriteLine(ex2); }
                        finally { Console.WriteLine(42);}
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("catch (Not",
                      @"catch (DivideByZeroException ex3)
                        { Console.WriteLine(ex3); }
                        catch (Not");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_using_statement_variable()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        using (var x = new Thing())
                        {
                        x = y + 2;
                        Console.WriteLine();
                        var x = 42;
                        }
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("x", "y");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }


      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_using_statement_initialization()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        using (conn.BeginTransaction())
                        {
                            x = y + 2;
                            Console.WriteLine();
                            var x = 42;
                        }
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("conn", "conAir");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_variable_initializer()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        var x = 42;
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("42", "43");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_variable_implicit()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        var x = 42;
                    }
            }";

         var csharpCodeChanged = csharpCode.ReplaceFirst("var", "int");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_variable_constant()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        int x = 42;
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("int", "const int");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_changed_variable()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        int x = 42;
                    }
            }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("int", "long");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_statement_levels_true_check()
      {
         var csharpCode = @"
                public class ClassA 
                {
                   public void Foo()
                   { 
                        y = z + 2;
                        var x = 42;
                    }
            }
            ";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var root2 = root1.Copy();

         var class1 = root1.Classes.First();
         var method1 = class1.Methods.First();
         var statement1 = method1.Statements.First();
         var assignment1 = statement1 as IAssignmentStatement;
         var expression1 = assignment1.Expression;
         var statement1b = method1.Statements.Last();
         var declaration1b = statement1b as IDeclarationStatement;
         var expression1b = declaration1b.Initializer;

         var class2 = root2.Classes.First();
         var method2 = class2.Methods.First();
         var statement2 = method2.Statements.First();
         var assignment2 = statement2 as IAssignmentStatement;
         var expression2 = assignment2.Expression;
         var statement2b = method2.Statements.Last();
         var declaration2b = statement2b as IDeclarationStatement;
         var expression2b = declaration2b.Initializer;

         Assert.IsTrue(expression1.SameIntent(expression2));
         Assert.IsTrue(expression1b.SameIntent(expression2b));
         Assert.IsTrue(assignment1.SameIntent(assignment2));
         Assert.IsTrue(declaration1b.SameIntent(declaration2b));
         Assert.IsTrue(statement1.SameIntent(statement2));
         Assert.IsTrue(statement1b.SameIntent(statement2b));
         Assert.IsTrue(method1.SameIntent(method2));
         Assert.IsTrue(class1.SameIntent(class2));
         Assert.IsTrue(root1.SameIntent(root2));
      }
      #endregion

      #region same intent expressions

      [TestMethod, TestCategory(SameIntentExpressionCategory)]
      public void Same_intent_true_with_same_expressions_in_different_contexts()
      {
         var csharpCode1 = @"
                namespace Namespace2
                {
                    public class Class1
                    { 
                        string Foo()
                        {
                            var x = y + 42;
                        } 
                    }
                }";
         var csharpCode2 = @"
                    public struct Structure1
                    { 
                        string Foo
                        { get{
                            var x = y + 42;
                            }
                        } 
                    }";
         var root1 = RDomCSharp.Factory.GetRootFromString(csharpCode1);
         var root2 = RDomCSharp.Factory.GetRootFromString(csharpCode2);
         var statement1 = root1.Descendants.OfType<IDeclarationStatement>().First();
         var statement2 = root2.Descendants.OfType<IDeclarationStatement>().First();
         var expr1 = statement1.Initializer;
         var expr2 = statement2.Initializer;
         Assert.IsTrue(expr1.SameIntent(expr2));
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_different_expressions()
      {
         var csharpCode = @"
                namespace Namespace2
                {
                    public class Class1
                    { 
                        string Foo()
                        {
                            var x = y + 42;
                        } 
                    }
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst(" y + 42", "Console.WriteLine()");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      [TestMethod, TestCategory(SameIntentStatementCategory)]
      public void Same_intent_false_with_different_variations_of_same_expressions()
      {
         var csharpCode = @"
                namespace Namespace2
                {
                    public class Class1
                    { 
                        string Foo()
                        {
                            var x = y + 42;
                        } 
                    }
                }";
         var csharpCodeChanged = csharpCode.ReplaceFirst("42", "3.14");
         AssertSameIntent(false, csharpCode, csharpCodeChanged);
      }

      #endregion

      #region  same intent special
      // These are primarily code coverage fix tests
      [TestMethod, TestCategory(SameIntentSpecialCategory)]
      public void Same_intent_false_with_different_types()
      {
         var csharpCode1 = @"
                namespace Namespace0
                {   }";
         var csharpCode2 = @"
                public interface Interface1 
                { }";
         AssertSameIntent(false, csharpCode1, csharpCode2);
      }

      [TestMethod, TestCategory(SameIntentSpecialCategory)]
      public void Same_intent_correct_when_called_directly()
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
         var annot1 = classes[0].PublicAnnotations.GetPublicAnnotation("kad_Test3");
         var annot2 = classes[1].PublicAnnotations.GetPublicAnnotation("kad_Test3");
         Assert.IsFalse(annot1.SameIntent(annot2));
         Assert.IsFalse(annot1.SameIntent(annot2, false));
         Assert.IsTrue(annot1.SameIntent(annot2, true));
      }

      [TestMethod, TestCategory(SameIntentSpecialCategory)]
      public void Same_intent_with_nulls()
      {
         var csharpCode = @"
            public class MyClass
            { }
            ";
         var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         var class1 = root.RootClasses.First();
         Assert.IsFalse(StandardSameIntent.CheckSameIntent(class1, null));
         Assert.IsFalse(StandardSameIntent.CheckSameIntent(null, class1));
         Assert.IsTrue(StandardSameIntent.CheckSameIntent<IClass>(null, null));
      }
      #endregion

      private void AssertSameIntent(bool shouldMatch, string initial, string changed, bool skipBuildSyntax = false)
      {
         var root1 = RDomCSharp.Factory.GetRootFromString(initial);
         var root1Copy = root1.Copy();
         Assert.IsTrue(root1.SameIntent(root1Copy), "Initial didn't match copy");
         var root2 = RDomCSharp.Factory.GetRootFromString(changed);
         var root2Copy = root2.Copy();
         Assert.IsTrue(root2.SameIntent(root2Copy), "Changed didn't match copy");
         Assert.AreEqual(shouldMatch, root1.SameIntent(root2), "Initial to Changed match wrong");
         if (!skipBuildSyntax)
         {
            var output1 = RDomCSharp.Factory.BuildSyntax(root1);
            Assert.AreEqual(initial, output1.ToFullString(), "BuildSyntax for initial didn't match");
            var output2 = RDomCSharp.Factory.BuildSyntax(root2);
            Assert.AreEqual(changed, output2.ToFullString(), "BuildSyntax for changed didn't match");
         }
      }

   }
}
