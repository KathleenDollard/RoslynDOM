using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using RoslynDom.Common;
using System.Linq;

namespace TestRoslyn
{
    [TestClass]
    public class GeneralUtilitiesTests
    {
        [TestMethod]
        public void GetAllCombos_returns_correct_for_empty()
        {
            var test = (new string[] { }).ToList();
            var result = test.GetAllCombos();
            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(x => string.Join(",", x) == ""));
        }

        [TestMethod]
        public void GetAllCombos_returns_correct_for_null()
        {
            var result = GeneralUtilities.GetAllCombos<object>(null);
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
            var test = (new string[] { "A", "B", "C", "D" }).ToList();
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

        [TestMethod]
        public void IsInteger_returns_correct_values()
        {
            sbyte x12 = 3; Assert.IsTrue(GeneralUtilities.IsInteger(x12));
            byte x1 = 3; Assert.IsTrue(GeneralUtilities.IsInteger(x1));
            short x2 = 3; Assert.IsTrue(GeneralUtilities.IsInteger(x2));
            ushort x3 = 3; Assert.IsTrue(GeneralUtilities.IsInteger(x3));
            int x4 = 3; Assert.IsTrue(GeneralUtilities.IsInteger(x4));
            float x5 = 3; Assert.IsFalse(GeneralUtilities.IsInteger(x5));
            double x6 = 3; Assert.IsFalse(GeneralUtilities.IsInteger(x6));
            decimal x7 = 3; Assert.IsFalse(GeneralUtilities.IsInteger(x7));
            DateTime x8 = new DateTime(); Assert.IsFalse(GeneralUtilities.IsInteger(x8));
            string x9 = "42"; Assert.IsFalse(GeneralUtilities.IsInteger(x9));
            bool x10 = true; Assert.IsFalse(GeneralUtilities.IsInteger(x10));
            Color x11 = Color.Red; Assert.IsTrue(GeneralUtilities.IsInteger(x11));
        }

        [TestMethod]
        public void IsFloatingPoint_returns_correct_values()
        {
            sbyte x12 = 3; Assert.IsFalse(GeneralUtilities.IsFloatingPint (x12));
            byte x1 = 3; Assert.IsFalse(GeneralUtilities.IsFloatingPint(x1));
            short x2 = 3; Assert.IsFalse(GeneralUtilities.IsFloatingPint(x2));
            ushort x3 = 3; Assert.IsFalse(GeneralUtilities.IsFloatingPint(x3));
            int x4 = 3; Assert.IsFalse(GeneralUtilities.IsFloatingPint(x4));
            float x5 = 3; Assert.IsTrue(GeneralUtilities.IsFloatingPint(x5));
            double x6 = 3; Assert.IsTrue(GeneralUtilities.IsFloatingPint(x6));
            decimal x7 = 3; Assert.IsFalse(GeneralUtilities.IsFloatingPint(x7));
            DateTime x8 = new DateTime(); Assert.IsFalse(GeneralUtilities.IsFloatingPint(x8));
            string x9 = "42"; Assert.IsFalse(GeneralUtilities.IsFloatingPint(x9));
            bool x10 = true; Assert.IsFalse(GeneralUtilities.IsFloatingPint(x10));
            Color x11 = Color.Red; Assert.IsFalse(GeneralUtilities.IsFloatingPint(x11));
        }

        private enum Color { Red, Green, Blue }
    }

}
