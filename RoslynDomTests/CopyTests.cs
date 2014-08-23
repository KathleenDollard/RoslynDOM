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
            VerifyClone(csharpCode, root => root.RootClasses.First().Attributes.Attributes.First());
        }

        private static void VerifyClone<T>(string csharpCode,Func<IRoot, IDom<T>> makeTestItem)
            where T : IDom<T>
        {
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var testItem = makeTestItem(root);
            var newAttribute = testItem.Copy();
            Assert.IsNotNull(newAttribute);
            Assert.IsTrue(newAttribute.SameIntent(testItem));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_attribute_value()
        {
            var csharpCode = @"
            [Foo(""Fred"", bar:3, bar2:""George"")] 
            public class Bar{}";
            VerifyClone(csharpCode, root => root.RootClasses.First().Attributes.Attributes.First().AttributeValues.Last());
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_method()
        {
            var csharpCode = @"
            public class Bar
            {
               public string Foo(int id, string firstName, string lastName) {}
            }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var method = root.RootClasses.First().Methods.First();
            var newMethod = method.Copy();
            Assert.IsNotNull(newMethod);
            Assert.IsTrue(newMethod.SameIntent(method));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_method_parameter()
        {
            var csharpCode = @"
            public class Bar
            {
               public string Foo(int id, string firstName, string lastName) {}
            }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var methodParameter = root.RootClasses.First().Methods.First().Parameters.First();
            var newMethodParameter = methodParameter.Copy();
            Assert.IsNotNull(newMethodParameter);
            Assert.IsTrue(newMethodParameter.SameIntent(methodParameter));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var method = root.RootClasses.First().Methods.First();
            var newMethod = method.Copy();
            Assert.IsNotNull(newMethod);
            Assert.IsTrue(newMethod.SameIntent(newMethod)); // test identity
            var statements = newMethod.Statements.ToArray();
            var rDomStatement = newMethod.Statements.First() as IStatement;
            Assert.IsTrue(statements[0] is RDomIfStatement);
            Assert.IsTrue(statements[1] is RDomDeclarationStatement);
            Assert.IsTrue(statements[2] is RDomAssignmentStatement);
            Assert.IsTrue(statements[3] is RDomReturnStatement);
            var outputOld = RDomCSharp.Factory.BuildSyntax(method);
            var outputNew = RDomCSharp.Factory.BuildSyntax(newMethod);
            Assert.AreEqual(outputOld.ToString(), outputNew.ToString());
            //var actual = RDomCSharp.Factory.BuildSyntax(root).ToFullString();
            //var expected = "public String Foo(Int32 id, String firstName, String lastName)\r\n{\r\n    if (true)\r\n    {\r\n    }\r\n\r\n    var x = \", \";\r\n    x = lastName + x + firstName;\r\n    return ret;\r\n}";
            Assert.AreEqual(cSharpMethodCode, outputOld.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var method = root.RootClasses.First().Methods.First();
            var newMethod = method.Copy();
            Assert.IsNotNull(newMethod);
            Assert.IsTrue(newMethod.SameIntent(newMethod)); // test identity
            var outputOld = RDomCSharp.Factory.BuildSyntax(method);
            var outputNew = RDomCSharp.Factory.BuildSyntax(newMethod);
            Assert.AreEqual(outputOld.ToString(), outputNew.ToString());
            Assert.AreEqual(cSharpMethodCode, outputOld.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var property = root.RootClasses.First().Properties.First();
            var newProperty = property.Copy();
            Assert.IsNotNull(newProperty);
            Assert.IsTrue(newProperty.SameIntent(property));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var property = root.RootClasses.First().Properties.First();
            var newProperty = property.Copy();
            Assert.IsNotNull(newProperty);
            Assert.IsTrue(newProperty.SameIntent(newProperty));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode , actual);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var property = root.RootClasses.First().Properties.First();
            var newProperty = property.Copy();
            Assert.IsNotNull(newProperty);
            Assert.IsTrue(newProperty.SameIntent(newProperty));
            Assert.IsNotNull(newProperty.SetAccessor);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_class()
        {
            var csharpCode = @"
            public class Bar
            {
               public string Foo{get; set;}
            }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var newClass = class1.Copy();
            Assert.IsNotNull(newClass);
            Assert.IsTrue(newClass.SameIntent(class1));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var structure = root.RootStructures.First();
            var newStructure = structure.Copy();
            Assert.IsNotNull(newStructure);
            Assert.IsTrue(newStructure.SameIntent(structure));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_interface()
        {
            var csharpCode = @"
            public interface Bar
            {
               string Foo{get; set;}
            }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var interface1 = root.RootInterfaces.First();
            var newInterface = interface1.Copy();
            Assert.IsNotNull(newInterface);
            Assert.IsTrue(newInterface.SameIntent(interface1));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
        }

        [TestMethod, TestCategory(CopyCategory)]
        public void Can_clone_enum()
        {
            var csharpCode = @"
            public enum Bar
            {
              Unknown, Red, Green, Blue
            }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var enum1 = root.RootEnums.First();
            var newEnum = enum1.Copy();
            Assert.IsNotNull(newEnum);
            Assert.IsTrue(newEnum.SameIntent(enum1));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var namespace1 = root.Namespaces.First();
            var newNamespace = namespace1.Copy();
            Assert.IsNotNull(newNamespace);
            Assert.IsTrue(newNamespace.SameIntent(namespace1));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var newRoot = root.Copy();
            Assert.IsNotNull(newRoot);
            Assert.IsTrue(newRoot.SameIntent(root));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var class1 = root.RootClasses.First();
            var newClass = class1.Copy();
            Assert.IsNotNull(newClass);
            Assert.IsTrue(newClass.SameIntent(class1));
            Assert.AreEqual(42, newClass.PublicAnnotations.GetValue("kad_Test3", "val2"));
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var actual = output.ToFullString();
            Assert.AreEqual(csharpCode, actual);
        }

        #endregion
    }
}
