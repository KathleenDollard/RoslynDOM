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
        [TestMethod ]
        public void Can_retrieve_if_descendants()
        {
            var csharpCode = @"
            public class Bar
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

            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var descendants = root.Descendants;
            Assert.AreEqual (23, descendants.Count());
        }

        [TestMethod]
        public void Can_retrieve_if_ancestors()
        {
            var csharpCode = @"
            public class Bar
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

            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var statement = root.Descendants.OfType<IAssignmentStatement>().First();
            var ancestors = statement.Ancestors;
            Assert.AreEqual(6, ancestors.Count());
        }


        [TestMethod]
        public void Can_retrieve_property_descendants()
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

            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var descendants = root.Descendants;
            Assert.AreEqual(10, descendants.Count());
        }

        [TestMethod]
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

            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var statement = root.Descendants.OfType<IAssignmentStatement>().First();
            var ancestors = statement.Ancestors;
            Assert.AreEqual(4, ancestors.Count());
        }
    }
}
