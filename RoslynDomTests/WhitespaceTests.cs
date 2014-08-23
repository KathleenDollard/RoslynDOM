using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass]
    public class WhitespaceTests
    {
        private const string WhitespaceCategory = "Whitespace";

        [TestMethod, TestCategory(WhitespaceCategory)]
        public void Recreate_whitespace_for_three_expected_if_styles()
        {
            var csharpCode = @"
public class Class1
{
    public void Foo()
    {
        var x = 3;
        if (x == 42){Console.WriteLine();}
        if (x == 42)
        {  Console.WriteLine(); }
        if (x == 42)
        {
            Console.WriteLine();
        }
    }
}
            ";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var statements = root.Classes.First().Methods.First().Statements.ToArray();
            var expected1 = "        if (x == 42){Console.WriteLine();}\r\n";
            var expected2 = "        if (x == 42)\r\n        {  Console.WriteLine(); }\r\n";
            var expected3 = "        if (x == 42)\r\n        {\r\n            Console.WriteLine();\r\n        }\r\n";
            var actual = RDomCSharp.Factory.BuildSyntax(root);
            var actual1 = RDomCSharp.Factory.BuildSyntax(statements[1]);
            var actual2 = RDomCSharp.Factory.BuildSyntax(statements[2]);
            var actual3 = RDomCSharp.Factory.BuildSyntax(statements[3]);
            // Problem was managing Whitespace trivia and NOT ALSO EOL trivia for each token. 
            // Partially fixed in CreateFrom. Either there is a bug there ors omethign is 
            // funny in BUildSyntax, which might be the presence of two WS.
            Assert.AreEqual(expected1, actual1.ToFullString());
            Assert.AreEqual(expected2, actual2.ToFullString());
            Assert.AreEqual(expected3, actual3.ToFullString());
        }
    }
}
