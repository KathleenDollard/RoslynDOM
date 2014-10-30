using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
   [TestClass]
   public class CopyTests
   {
      private const string SameIntentTestCategory = "SameIntent";
      private const string CopyCategory = "Copy";
      private const string CopyMethodsCategory = "CopyMethods";

      #region clone tests
      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_attribute()
      {
         var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2:""George"")] 
            public class Bar{}";
         VerifyClone(csharpCode,
             root => root.RootClasses.First().Attributes.Attributes.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_attribute_value()
      {
         var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2:""George"")] 
            public class Bar{}";
         VerifyClone(csharpCode,
             root => root.RootClasses.First().Attributes.Attributes.First().AttributeValues.Last());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_constructor()
      {
         var csharpCode = @"
            public class Bar
            {
                public Bar()
                {}
                public static Bar()
                {}
            }";
         VerifyClone(csharpCode,
             root => root.RootClasses.First().Constructors.First(),
             x => x.Replace("unknown_name", "Bar"));

         VerifyClone(csharpCode,
             root => root.RootClasses.First().Constructors.ElementAt(1),
             x => x.Replace("unknown_name", "Bar"));
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_destructor()
      {
         var csharpCode = @"
            public class Complex
            {
                public static Complex operator +(Complex c1, Complex c2)
                {
                     return new Complex(c1.real + c2.real, c1.imaginary + c2.imaginary);
                }
            }";
         Assert.Inconclusive();
         //VerifyClone(csharpCode,
         //    root => root.RootClasses.First().Destructors.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_plus_operator()
      {
         var csharpCode = @"
            public class Complex
            {
                public static Complex operator +(Complex c1, Complex c2)
                {
                     return new Complex(c1.real + c2.real, c1.imaginary + c2.imaginary);
                }
            }";
         VerifyClone(csharpCode,
             root => root.RootClasses.First().Operators.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_minus_operator()
      {
         var csharpCode = @"
            public class Complex
            {
                public static Complex operator -(Complex c1, Complex c2)
                {
                     return new Complex(c1.real - c2.real, c1.imaginary - c2.imaginary);
                }
            }";
         VerifyClone(csharpCode,
             root => root.RootClasses.First().Operators.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_explicit_conversion_operator()
      {
         var csharpCode = @"
         public class Digit
         {
            public static explicit operator Digit(byte b)
            {
               Digit d = new Digit(b);
               return d;
            }
         }";
         VerifyClone(csharpCode,
             root => root.RootClasses.First().ConversionOperators .First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_implicit_conversion_operator()
      {
         var csharpCode = @"
         public class Digit
         {
            public static implicit operator Digit(byte b)
            {
               Digit d = new Digit(b);
               return d;
            }
         }";
         VerifyClone(csharpCode,
              root => root.RootClasses.First().ConversionOperators.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_method()
      {
         var csharpCode = @"
            public class Bar
            {
               public string Foo(int id, string firstName, string lastName) {}
            }";
         VerifyClone(csharpCode,
             root => root.RootClasses.First().Methods.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_static_method()
      {
         var csharpCode = @"
            public static class Bar
            {
               public static string Foo(int id, string firstName, string lastName) {}
            }";
         VerifyClone(csharpCode,
             root => root.RootClasses.First().Methods.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_region()
      {
         var csharpCode = @"
            // Comment
            public class Bar
            {
                #region This is a region
                public Bar()
                {}
                #endregion
                public static Bar()
                {}
            }";
         var root = RDom.CSharp.Load(csharpCode);
         var region = root.Classes.First().MembersAll.OfType<IDetailBlockStart>().First();
         Assert.IsNotNull(region.BlockEnd);
         var root2 = root.Copy();
         var region2 = root2.Classes.First().MembersAll.OfType<IDetailBlockStart>().First();
         Assert.IsNotNull(region2.BlockEnd);
         VerifyClone(csharpCode,
             r => r.RootClasses.First().MembersAll.OfType<IDetailBlockStart>().First());
      }


      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_method_parameter()
      {
         var csharpCode = @"
            public class Bar
            {
               public string Foo(int id, string firstName, string lastName) {}
            }";
         VerifyClone(csharpCode,
            root => root.RootClasses.First().Methods.First().Parameters.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_method_parameter_not_implicit()
      {
         var csharpCode = @"
            public class Bar
            {
               public string Foo(int id, String firstName, string lastName) {}
            }";
         VerifyClone(csharpCode,
            root => root.RootClasses.First().Methods.First().Parameters.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_method_with_body()
      {
         var cSharpMethodCode =
@"               public string Foo(int id, string firstName, string lastName)
                {
                  if  ( true )  {}
                  var x = "", "";
                  String y = x;
                  x = lastName + x + firstName;
                  return ret;
                }
";
         var csharpCode =
@"            public class Bar
            {
" + cSharpMethodCode +
@"            }";
         var newItem = VerifyClone(csharpCode,
            root => root.RootClasses.First().Methods.First());
         var newMethod = newItem as IMethod;
         var statements = newMethod.Statements.ToArray();
         var rDomStatement = newMethod.Statements.First() as IStatement;
         Assert.IsTrue(statements[0] is RDomIfStatement);
         Assert.IsTrue(statements[1] is RDomDeclarationStatement);
         Assert.IsTrue(statements[2] is RDomDeclarationStatement);
         Assert.IsTrue(statements[3] is RDomAssignmentStatement);
         Assert.IsTrue(statements[4] is RDomReturnStatement);
         var outputNew = RDom.CSharp.GetSyntaxNode(newItem);
         Assert.AreEqual(cSharpMethodCode, outputNew.ToFullString());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_method_with_array_return_type()
      {
         var cSharpMethodCode =
@"               public string[] Foo(int id, string firstName, string lastName)
                {  }
";
         var csharpCode =
@"            public class Bar
            {
" + cSharpMethodCode +
@"            }";
         var newItem = VerifyClone(csharpCode,
            root => root.RootClasses.First().Methods.First());
         var outputNew = RDom.CSharp.GetSyntaxNode(newItem);
         Assert.AreEqual(cSharpMethodCode, outputNew.ToFullString());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_property()
      {
         var csharpCode = @"
            public class Bar
            {
               public string Foo{  get ; set ; }
            }";
         VerifyClone(csharpCode,
            root => root.RootClasses.First().Properties.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_property_get_with_body()
      {
         var csharpCode = @"
            public class Bar
            {
               private string firstName;
               private string lastName;
               public string Foo
               { get {
                  var x = "", "";
                  var ret = lastName + x + firstName;
                  return ret;
               } }
            }";
         var newItem = VerifyClone(csharpCode,
                 root => root.RootClasses.First().Properties.First());
         var newProperty = newItem as IProperty;
         Assert.IsNotNull(newProperty.GetAccessor);

      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_property_set_with_body()
      {
         var csharpCode = @"
            public class Bar
            {
               private string firstName;
               private string firstName;
               public string Foo
               { 
                 get 
                 {
                   var x = "", "";
                   var ret = lastName + x + firstName;
                   return ret;
                 } 
                 set 
                 {
                   var x = "", "";
                   lastName = x + firstName;
                 } 
               }
            }";
         var newItem = VerifyClone(csharpCode,
                  root => root.RootClasses.First().Properties.First());
         var newProperty = newItem as IProperty;
         Assert.IsNotNull(newProperty.SetAccessor);
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_class()
      {
         var csharpCode = @"
            public class Bar
            {
               public string Foo{get; set;}
            }";
         VerifyClone(csharpCode,
                    root => root.RootClasses.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_structure()
      {
         var csharpCode = @"
            public struct Bar
            {
               public string Foo{get; set;}
               public string Foo2(int FooBar) {}
            }";
         VerifyClone(csharpCode,
                    root => root.RootStructures.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_interface()
      {
         var csharpCode = @"
            public interface Bar
            {
               string Foo{get; set;}
            }";
         VerifyClone(csharpCode,
                    root => root.RootInterfaces.First());
      }

      [TestMethod, TestCategory(CopyCategory)]
      public void Can_clone_enum()
      {
         var csharpCode = @"
            public enum Bar
            {
              Unknown, Red, Green, Blue
            }";
         VerifyClone(csharpCode,
                 root => root.RootEnums.First());
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
            }";
         VerifyClone(csharpCode,
                    root => root.Namespaces.First());
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
                    private string Foo2(int George) {}
                }  
            }";
         VerifyClone(csharpCode, root => root);
      }

      #endregion
      private IDom<T> VerifyClone<T>(string csharpCode,
          Func<IRoot, IDom<T>> makeTestItem,
          Func<string, string> fixupOutput = null)
          where T : IDom<T>
      {
         var root = RDom.CSharp.Load(csharpCode);
         var testItem = makeTestItem(root);
         var newItem = testItem.Copy();
         Assert.IsNotNull(newItem);
         Assert.IsTrue(newItem.SameIntent(testItem));
         var output = RDom.CSharp.GetSyntaxNode(root);
         var actual = output.ToFullString();
         Assert.AreEqual(csharpCode, actual);
         var outputOldText = RDom.CSharp.GetSyntaxNode(testItem).ToString();
         var outputNewText = RDom.CSharp.GetSyntaxNode(newItem).ToString();
         if (fixupOutput != null)
         { outputNewText = fixupOutput(outputNewText); }
         Assert.AreEqual(outputOldText, outputNewText);
         return newItem;
      }

   }
}
