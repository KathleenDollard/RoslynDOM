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
        public void Can_clone_method_with_body()
        {
            var cSharpMethodCode =
@"               public string Foo(int id, string firstName, string lastName)
                {
                  if  ( true )  {}
                  var x = "", "";
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
            Assert.IsTrue(statements[2] is RDomAssignmentStatement);
            Assert.IsTrue(statements[3] is RDomReturnStatement);
            var outputNew = RDomCSharp.Factory.BuildSyntax(newItem);
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
            var outputNew = RDomCSharp.Factory.BuildSyntax(newItem);
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
                    private string Foo { get; set; }
                }  
            }         
            namespace Namespace2
            {
                public class Bar2
                {
                    string Foo2(int George) {}
                }  
            }";
            VerifyClone(csharpCode, root => root);
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
            }";
            var newItem = VerifyClone(csharpCode,
                     root => root.RootClasses.First());
            var newClass = newItem as IClass;
            Assert.AreEqual(42, newClass.PublicAnnotations.GetValue("kad_Test3", "val2"));
        }

        #endregion
        private IDom<T> VerifyClone<T>(string csharpCode,
            Func<IRoot, IDom<T>> makeTestItem)
            where T : IDom<T>
        {
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var testItem = makeTestItem(root);
            var newItem = testItem.Copy();
            Assert.IsNotNull(newItem);
            Assert.IsTrue(newItem.SameIntent(testItem));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
            var outputOld = RDomCSharp.Factory.BuildSyntax(testItem);
            var outputNew = RDomCSharp.Factory.BuildSyntax(newItem);
            Assert.AreEqual(outputOld.ToString(), outputNew.ToString());
            return newItem;
        }

    }
}
