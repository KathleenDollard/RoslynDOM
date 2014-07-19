using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;

namespace RoslynDomTests
{
    [TestClass]
    public class AncestorsAndDependentsTests
    {
        [TestMethod ]
        public void Can_retrieve_if_descendant()
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
            Assert.Inconclusive();
        }
    }
}
