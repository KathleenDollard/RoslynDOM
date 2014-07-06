using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;

namespace RoslynDomTests
{
    [TestClass]
    public class DiagnosticsTests
    {
        private const string DiagnosticsTestsCategory = "DiagnosticsTests";

        [TestMethod ]
        [TestCategory(DiagnosticsTestsCategory)]
        public void No_diagnostics_for_correct_happy_case()
        {
            var csharpCode = @"
            //[[root:kad_Test1()]]
            public class MyClass
            {
                public string Foo{get; set;} 
            }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsFalse(root.HasSyntaxErrors);

        }

        [TestMethod]
        [TestCategory(DiagnosticsTestsCategory)]
        public void Find_diagnostics_issue_for_unhappy_case()
        {
            var csharpCode = @"
            //[[root:kad_Test1()]]
            public class MyClass
            {
                public string Foo(getx; set;} 
            }
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsTrue(root.HasSyntaxErrors);

        }
    }
}
