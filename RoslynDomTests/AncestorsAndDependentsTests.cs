using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
   [TestClass]
   public class AncestorsAndDependentsTests
   {
      private const string StatementHierarchyCategory = "StatementHierarchy";
      private const string EntityHierarchyCategory = "EntityHierarchy";
      private const string HierarchyReportingCategory = "HierarchyReporting";

      #region statement hierarchy
      [TestMethod, TestCategory(StatementHierarchyCategory)]
      public void Can_retrieve_if_descendants_descendants_and_children()
      {
         var csharpCode =
       @"public class Bar
            {
                public void Foo()
                {
                    if (z == 1)
                    {
                        var x = 42;
                    }
                    else if (z==2)
                    { var x = 43;  y = x + x; }
                    else
                    { Console.WriteLine(""Fred""); }
                    if (z == 1) Console.WriteLine(""George"");
                    if (z == 2) Console.Write(""Sam"");
                }
            }";
         // descendants
         var rootDescendantCount = 23;
         //var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         //var descendants = root.Descendants;
         //Assert.AreEqual(23, descendants.Count());

         //IEnumerable<IDom> commonAncestors = new List<IDom>()
         //{
         //    root,
         //    root.Classes.First(),
         //    root.Classes.First().Methods.First()
         //};

         // if
         //var ifStatement = root.Descendants.OfType<IIfStatement>().First();
         VerifyDescendantsAndAncestors
             (csharpCode,
               r => r.Descendants.OfType<IIfStatement>().First(),
               r => r.Classes.First().Methods.First(),
               "if (z == 1)",
              rootDescendantCount, 3, 3, 4, 12, "If");

         // else 
         VerifyDescendantsAndAncestors
             (csharpCode,
               r => r.Descendants.OfType<IFinalElseStatement>().First(),
               r => r.Classes.First().Methods.First(),
               null,
               rootDescendantCount, 3, 4, 1, 2, "Final else");

         // else if children
         VerifyDescendantsAndAncestors
           (csharpCode,
               r => r.Descendants.OfType<IElseIfStatement>().First(),
               r => r.Classes.First().Methods.First(),
               null,
               rootDescendantCount, 3, 4, 3, 5, "Else if");

         // if appears as one child
         //var method = root.Descendants.OfType<IMethod>().First();
         //Assert.AreEqual(3, method.Children.Count());

      }

      [TestMethod, TestCategory(StatementHierarchyCategory)]
      public void Can_retrieve_try_descendants_ancestors_and_children()
      {
         var csharpCode =
       @"public class Bar
            {
                public void Foo()
                {
                    try
                    {
                        var x = 42;
                    }
                    catch (DivideByZeroException ex)
                    { var x = 43;  y = x + x; }
                    catch (Exception ex)
                    { Console.WriteLine(""Fred""); }
                    finally
                    { var z = 2;}
                }
            }";
         // descendants
         var rootDescendantCount = 18;
         //var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
         //var descendants = root.Descendants;
         //Assert.AreEqual(16, descendants.Count());

         //IEnumerable<IDom> commonAncestors = new List<IDom>()
         //{
         //    root,
         //    root.Classes.First(),
         //    root.Classes.First().Methods.First()
         //};

         // try
         //var tryStatement = root;
         VerifyDescendantsAndAncestors
             (csharpCode,
               r => r.Descendants.OfType<ITryStatement>().First(),
               r => r.Classes.First().Methods.First(),
               "",
               rootDescendantCount, 1, 3, 4, 15, "Try");

         // catch 1 
         VerifyDescendantsAndAncestors
             (csharpCode,
               r => r.Descendants.OfType<ICatchStatement>().First(),
               r => r.Classes.First().Methods.First(),
               null,
               rootDescendantCount, 1, 4, 3, 5, "Catch 1");

         // catch 2
         VerifyDescendantsAndAncestors
              (csharpCode,
               r => r.Descendants.OfType<ICatchStatement>().Skip(1).First(),
               r => r.Classes.First().Methods.First(),
               null,
               rootDescendantCount, 1, 4, 2, 3, "Catch 2");

         // finally
         VerifyDescendantsAndAncestors
              (csharpCode,
               r => r.Descendants.OfType<IFinallyStatement>().First(),
               r => r.Classes.First().Methods.First(),
               null,
               rootDescendantCount, 1, 4, 1, 2, "Finally");

         //// if appears as one child
         //var method = root.Descendants.OfType<IMethod>().First();
         //Assert.AreEqual(1, method.Children.Count());

         //var output = RDomCSharp.Factory.BuildSyntax(root).ToFullString();
         //Assert.AreEqual(csharpCode, output, "BuildSyntax doesn't match");
      }

      #endregion

      #region entity hierarchy
      [TestMethod, TestCategory(EntityHierarchyCategory)]
      public void Can_retrieve_class_descendants_ancestors_and_children()
      {
         var csharpCode =
           @"public class Bar
                {
                    public string FooBar
                    {
                       get
                        {
                            ushort z = 432;
                            return z.ToString();
                        }
                        set
                        {
                            xyz = value;
                        }
                    }
                }";

         VerifyDescendantsAndAncestors(
               csharpCode,
               r => r.Classes.First(),
               r => r,
               "public class Bar",
               10, 1, 1, 1, 9, "Class:");

      }

      [TestMethod, TestCategory(EntityHierarchyCategory)]
      public void Can_retrieve_struct_descendants_ancestors_and_children()
      {
         var csharpCode =
           @"public struct Bar
                {
                    public string FooBar
                    {
                       get
                        {
                            ushort z = 432;
                            return z.ToString();
                        }
                        set
                        {
                            xyz = value;
                        }
                    }
                }";

         VerifyDescendantsAndAncestors(
               csharpCode,
               r => r.Structures.First(),
               r => r,
               "public struct Bar",
               10, 1, 1, 1, 9, "Structure:");

      }

      [TestMethod, TestCategory(EntityHierarchyCategory)]
      public void Can_retrieve_interface_descendants_ancestors_and_children()
      {
         var csharpCode =
           @"public interface Bar
                {
                    public string FooBar{get; set; }
                }";

         VerifyDescendantsAndAncestors(
               csharpCode,
               r => r.Interfaces.First(),
               r => r,
               "public interface Bar",
               4, 1, 1, 1, 3, "Interface:");

      }

      [TestMethod, TestCategory(EntityHierarchyCategory)]
      public void Can_retrieve_enum_descendants_ancestors_and_children()
      {
         var csharpCode =
           @"public enum Bar
                {
                    Foo, Baz
                }";

         VerifyDescendantsAndAncestors(
               csharpCode,
               r => r.Enums.First(),
               r => r,
               "public enum Bar",
               3, 1, 1, 2, 2, "Enum:");
      }

      [TestMethod, TestCategory(EntityHierarchyCategory)]
      public void Can_retrieve_property_descendants_ancestors_and_children()
      {
         var csharpCode =
           @"public class Bar
                {
                    public string FooBar
                    {
                       get
                        {
                            ushort z = 432;
                            return z.ToString();
                        }
                        set
                        {
                            xyz = value;
                        }
                    }
                }";
         VerifyDescendantsAndAncestors(
               csharpCode,
               r => r.Classes.First().Properties.First(),
               r => r.Classes.First(),
               "public string FooBar",
               10, 1, 2, 2, 8, "Property:");
      }


      [TestMethod, TestCategory(EntityHierarchyCategory)]
      public void Can_retrieve_method_descendants_ancestors_and_children()
      {
         var csharpCode =
           @"public class Bar
                {
                    public string FooBar()
                    {
                        ushort z = 432;
                        xyz = value;
                        return z.ToString();
                    }
                }";
         VerifyDescendantsAndAncestors(
               csharpCode,
               r => r.Classes.First().Methods.First(),
               r => r.Classes.First(),
               "public string FooBar",
               8, 1, 2, 3, 6, "Method:");
      }


      [TestMethod, TestCategory(EntityHierarchyCategory)]
      public void Can_retrieve_constructor_descendants_ancestors_and_children()
      {
         var csharpCode =
           @"public class Bar
                {
                    public Bar()
                    {
                        ushort z = 432;
                        xyz = value;
                    }
                }";
         VerifyDescendantsAndAncestors(
               csharpCode,
               r => r.Classes.First().Constructors.First(),
               r => r.Classes.First(),
               "public Bar",
               6, 1, 2, 2, 4, "Constructor:");
      }


      #endregion

      #region hierarchy reporting
      [TestMethod, TestCategory(HierarchyReportingCategory)]
      public void Can_report_report_simple_hierarchy()
      {
         var csharpCode = @"
            public class Bar
            {
               public string FooBar
               {
                    get
                    {
                        ushort z = 432;
                        return z.ToString();
                    }
                    set
                    {
                        xyz = value;
                    }
                }
            }           
            ";
         Console.WriteLine(csharpCode);
         var root = RDom.CSharp.Load(csharpCode);
         var actual = root.ReportHierarchy();
         var expected = "RoslynDom.RDomRoot : <root>\r\n  RoslynDom.RDomVerticalWhitespace : \r\n  RoslynDom.RDomClass : Bar\r\n    RoslynDom.RDomProperty : FooBar\r\n      RoslynDom.RDomPropertyAccessor : get_FooBar\r\n        RoslynDom.RDomDeclarationStatement : z {UInt16}\r\n          RoslynDom.RDomOtherExpression : 432\r\n        RoslynDom.RDomReturnStatement : \r\n          RoslynDom.RDomInvocationExpression : z.ToString()\r\n      RoslynDom.RDomPropertyAccessor : set_FooBar\r\n        RoslynDom.RDomAssignmentStatement : \r\n          RoslynDom.RDomOtherExpression : value\r\n";
         Assert.AreEqual(expected, actual);

         expected = "RoslynDom.RDomClass : Bar";
         var cl = root.Descendants.OfType<IClass>().First();
         Assert.AreEqual(expected, cl.ToString());
      }


      #endregion

      private void VerifyDescendantsAndAncestors
       (string csharpCode,
               Func<IRoot, IDom> itemDelegate,
               Func<IRoot, IDom> itemParentDelegate,
               string syntaxStringStart,
               int rootDescendantCount,
               int parentChildCount,
               int ancestorCount,
               int childCount,
               int descendantCount,
               string assertId)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var item = itemDelegate(root);
         var itemParent = itemParentDelegate(root);
         if (!string.IsNullOrWhiteSpace(assertId)) assertId += ": ";
         if (syntaxStringStart != null)
         {
            var str = RDom.CSharp.GetSyntaxNode(item).ToString();
            Assert.AreEqual(syntaxStringStart, str.Substring(0, syntaxStringStart.Length), assertId + "Syntax strings don't match");
         }
         Assert.AreEqual(rootDescendantCount, root.Descendants.Count(), assertId + "Root descendants wrong");
         Assert.AreEqual(parentChildCount, itemParent.Children.Count(), assertId + "Item parent's child count wrong");
         Assert.AreEqual(ancestorCount, item.Ancestors.Count(), assertId + "Ancestor count wrong");
         Assert.AreEqual(childCount, item.Children.Count(), assertId + "Child count wrong");
         Assert.AreEqual(descendantCount, item.Descendants.Count(), assertId + "Descendant count wrong");

         var output = RDom.CSharp.GetSyntaxNode(root).ToFullString();
         Assert.AreEqual(csharpCode, output, "BuildSyntax doesn't match");
      }

      //private void AssertDescendantsAndAncestors
      //         (IDom item,
      //         string syntaxStringStart,
      //         IEnumerable<IDom> ancestors,
      //         int childCount,
      //         int descendantCount,
      //         string assertId)
      //{
      //    if (!string.IsNullOrWhiteSpace(assertId)) assertId += ": ";
      //    if (syntaxStringStart != null)
      //    {
      //        var str = RDomCSharp.Factory.BuildSyntax(item).ToString();
      //        Assert.AreEqual(syntaxStringStart, str.Substring(0, syntaxStringStart.Length), assertId + "Syntax strings don't match");
      //    }
      //    var expected = ancestors.Reverse().ToArray();
      //    var actual = item.Ancestors.ToArray();
      //    Assert.AreEqual(expected.Count(), actual.Count(), assertId + "Ancestor count doesn't match");
      //    for (int i = 0; i < actual.Length; i++)
      //    { Assert.AreEqual(expected[i], actual[i], assertId + "Ancestor[" + i + "] doesn't match"); }
      //    Assert.AreEqual(childCount, item.Children.Count(), assertId + "Child count wrong");
      //    Assert.AreEqual(descendantCount, item.Descendants.Count(), assertId + "Descendant count wrong");
      //}
   }
}
