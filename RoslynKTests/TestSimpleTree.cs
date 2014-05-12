using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KadGen.Common;

namespace TestRoslyn
{
    [TestClass]
    public class TestSimpleTree
    {
        [TestMethod ]
        public void Should_correctly_nest_strings()
        {
            var source = new string[] { "A" ,"AB", "ABC","AC", "B", "C", "CD", "D"};
            Func<string, string, string, bool> func = (x, xPrev, xNext)=>x.StartsWith(xPrev);
            var tree = SimpleTreeNode<string>.CreateTreeFromList(source, func);
            var report = "   \r\n      A\r\n         AB\r\n            ABC\r\n         AC\r\n      B\r\n      C\r\n         CD\r\n      D\r\n";
            Assert.AreEqual(report, tree.Report("   "));
        }

        [TestMethod]
        public void Should_load_list()
        {
            var source = new string[] { "A",  "B", "C",  "D" };
            Func<string, string, string, bool> func = (x, xPrev, xNext) => x.StartsWith(xPrev);
            var tree = SimpleTreeNode<string>.CreateTreeFromList(source, func);
            Assert.AreEqual(4, tree.Children.Count());
        }
    }
}
