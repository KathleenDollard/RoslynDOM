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
    public class StatementTests
    {
        private const string CodeLoadCategory = "CodeLoad";

        [TestMethod, TestCategory(CodeLoadCategory)]
        public void Can_load_statements_for_method()
        {
            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                  if (true) {}
                  var x = "", "";
                  x = lastName + x + firstName;
                  return true;
                }
            }           
            ";
            var root = RDomFactory.GetRootFromString(csharpCode) ;
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(4, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomIfStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomDeclarationStatement));
            Assert.IsInstanceOfType(statements[2], typeof(RDomAssignmentStatement));
            Assert.IsInstanceOfType(statements[3], typeof(RDomReturnStatement));
            var output = ((RDomRoot)root).BuildSyntax();
            Assert.Inconclusive();
            //Assert.IsFalse(classes[0] == classes[1]); // reference equality fails
            //Assert.IsTrue(classes[0].SameIntent(classes[1]));
        }
    }
}
