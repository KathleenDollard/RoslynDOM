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
            }           
            ";
            // descendants
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var descendants = root.Descendants;
            Assert.AreEqual(23, descendants.Count());

            // ancestors
            var statement = root.Descendants.OfType<IAssignmentStatement>().First();
            Assert.AreEqual(5, statement.Ancestors.Count());

            // if children
            var ifStatement = root.Descendants.OfType<IIfStatement>().First();
            Assert.AreEqual(4, ifStatement.Children.Count());

            // if appears as one child
            var method = root.Descendants.OfType<IMethod>().First();
            Assert.AreEqual(3, method.Children.Count());

        }
        #endregion

        #region entity hierarchy
        [TestMethod, TestCategory(EntityHierarchyCategory)]
        public void Can_retrieve_property_descendants()
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
                }
            ";

            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var descendants = root.Descendants;
            Assert.AreEqual(10, descendants.Count());
        }

        [TestMethod, TestCategory(EntityHierarchyCategory)]
        public void Can_retrieve_property_ancestors()
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

            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var statement = root.Descendants.OfType<IAssignmentStatement>().First();
            var ancestors = statement.Ancestors;
            Assert.AreEqual(4, ancestors.Count());
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

            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var actual = root.ReportHierarchy();
            var expected = "RoslynDom.RDomRoot : Root\r\n  RoslynDom.RDomVerticalWhitespace : \r\n  RoslynDom.RDomClass : Bar\r\n    RoslynDom.RDomProperty : FooBar\r\n      RoslynDom.RDomPropertyAccessor : get_FooBar\r\n        RoslynDom.RDomDeclarationStatement : z {UInt16}\r\n          RoslynDom.RDomExpression : 432\r\n        RoslynDom.RDomReturnStatement : \r\n          RoslynDom.RDomExpression : z.ToString()\r\n      RoslynDom.RDomPropertyAccessor : set_FooBar\r\n        RoslynDom.RDomAssignmentStatement : \r\n          RoslynDom.RDomExpression : value\r\n";
            Assert.AreEqual(expected, actual);

            expected = "RoslynDom.RDomClass : Bar";
            var cl = root.Descendants.OfType<IClass>().First();
            Assert.AreEqual(expected, cl.ToString());
        }


        #endregion

    }
}
