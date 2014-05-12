using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using KadGen.Common;
using System.Linq;

namespace TestRoslyn
{
    [TestClass]
    public class TestGeneralUtilities
    {
        [TestMethod]
        public void GetAllCombos_returns_correct_for_empty()
        {
            var test = (new string[] {  }).ToList();
            var result = test.GetAllCombos();
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(x => string.Join(",", x) == ""));
        }

        [TestMethod]
        public void GetAllCombos_returns_correct_for_null()
        {
            var result = GeneralUtilities .GetAllCombos<object>(null);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetAllCombos_returns_correct_for_A()
        {
            var test = (new string[] { "A" }).ToList();
            var result = test.GetAllCombos();
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(x => string.Join(",", x) == ""));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A"));
        }
        [TestMethod]
        public void GetAllCombos_returns_correct_for_AB()
        {
            var test = (new string[] { "A", "B" }).ToList();
            var result = test.GetAllCombos();
            Assert.AreEqual(4, result.Count());
            Assert.IsTrue(result.Any(x => string.Join(",", x) == ""));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B"));
        }
        [TestMethod]
        public void GetAllCombos_returns_correct_for_ABC()
        {
            var test = (new string[] { "A", "B", "C" }).ToList();
            var result = test.GetAllCombos();
            Assert.AreEqual(8, result.Count());
            Assert.IsTrue(result.Any(x => string.Join(",", x) == ""));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "C"));
        }
        [TestMethod]
        public void GetAllCombos_returns_correct_for_ABCD()
        {
            var test =(new string[] { "A", "B", "C", "D" }).ToList();
            var result = test.GetAllCombos();
            Assert.AreEqual(16, result.Count());
            Assert.IsTrue(result.Any(x => string.Join(",", x) == ""));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,C,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,C,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,C,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "C,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "D"));
        }

        [TestMethod]
        public void GetAllCombos_returns_correct_for_ABCDE()
        {
            var test = (new string[] { "A", "B", "C", "D", "E" }).ToList();
            var result = test.GetAllCombos();
            Assert.AreEqual(32, result.Count());
            Assert.IsTrue(result.Any(x => string.Join(",", x) == ""));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,C,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,C,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,D,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,C,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,C,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,D,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,C,D,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "A,B,C,D,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,C,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,C,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,D,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "B,C,D,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "C"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "C,D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "C,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "C,D,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "D"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "D,E"));
            Assert.IsTrue(result.Any(x => string.Join(",", x) == "E"));
        }
    }
}
