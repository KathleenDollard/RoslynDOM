using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common; 

namespace TestRoslyn
{
    [TestClass]
    public class StringUtilitiesTests
    {
        private string singleIndex = "1234.56";
        private string multipleIndex = "1.234.56.234.56";
        private string endIndex = "123456.";
        private string startIndex = ".123456";
        private string noIndex = "123456";
        private string emptyString = "";

        [TestMethod]
        public void SubstringAfter_finds_part_after_delimiter()
        {
            Assert.AreEqual("56", singleIndex.SubstringAfter("."));
        }

        [TestMethod]
        public void SubstringAfter_finds_part_after_multichar_delimiter()
        {
            Assert.AreEqual("4.56", singleIndex.SubstringAfter("23"));
        }

        [TestMethod]
        public void SubstringAfter_finds_part_after_first_delimiter_with_multiples()
        {
            Assert.AreEqual("234.56.234.56", multipleIndex.SubstringAfter("."));
        }

        [TestMethod]
        public void SubstringAfter_finds_part_after_first_multichar_delimiter_with_multiples()
        {
            Assert.AreEqual("4.56.234.56", multipleIndex.SubstringAfter(".23"));
        }

        [TestMethod]
        public void SubstringAfter_returns_empty_string_if_nothing_after_delimiter()
        {
            Assert.AreEqual("", endIndex.SubstringAfter("."));
        }

        [TestMethod]
        public void SubstringAfter_returns_empty_string_if_nothing_after_multichar_delimiter()
        {
            Assert.AreEqual("", endIndex.SubstringAfter("56."));
        }

        [TestMethod]
        public void SubstringAfter_returns_empty_string_if_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringAfter("."));
        }

        [TestMethod]
        public void SubstringAfter_returns_empty_string_if_multichar_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringAfter(".23"));
        }

        [TestMethod]
        public void SubstringAfter_returns_null_for_null()
        {
            Assert.IsNull ( StringUtilities.SubstringAfter(null,"."));
        }

        [TestMethod]
        public void SubstringAfterLast_finds_part_after_delimiter()
        {
            Assert.AreEqual("56", singleIndex.SubstringAfterLast("."));
        }

        [TestMethod]
        public void SubstringAfterLast_finds_part_after_multichar_delimiter()
        {
            Assert.AreEqual("4.56", singleIndex.SubstringAfterLast("23"));
        }

        [TestMethod]
        public void SubstringAfterLast_finds_part_after_last_delimiter_with_multiples()
        {
            Assert.AreEqual("56", multipleIndex.SubstringAfterLast("."));
        }

        [TestMethod]
        public void SubstringAfterLast_finds_part_after_last_multichar_delimiter_with_multiples()
        {
            Assert.AreEqual("4.56", multipleIndex.SubstringAfterLast(".23"));
        }

        [TestMethod]
        public void SubstringAfterLast_returns_empty_string_if_nothing_after_delimiter()
        {
            Assert.AreEqual("", endIndex.SubstringAfterLast("."));
        }

        [TestMethod]
        public void SubstringAfterLast_returns_empty_string_if_nothing_after_multichar_delimiter()
        {
            Assert.AreEqual("", endIndex.SubstringAfterLast("56."));
        }

        [TestMethod]
        public void SubstringAfterLast_returns_empty_string_if_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringAfterLast("."));
        }

        [TestMethod]
        public void SubstringAfterLast_returns_empty_string_if_multichar_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringAfterLast(".23"));
        }

        [TestMethod]
        public void SubstringAfterLast_returns_null_for_null()
        {
            Assert.IsNull(StringUtilities.SubstringAfterLast(null, "."));
        }

        [TestMethod]
        public void SubstringBefore_finds_part_before_delimiter()
        {
            Assert.AreEqual("1234", singleIndex.SubstringBefore("."));
        }

        [TestMethod]
        public void SubstringBefore_finds_part_before_first_delimiter_with_multiples()
        {
            Assert.AreEqual("1", multipleIndex.SubstringBefore("."));
        }

        [TestMethod]
        public void SubstringBefore_returns_empty_string_if_nothing_before_delimiter()
        {
            Assert.AreEqual("", startIndex.SubstringBefore("."));
        }

        [TestMethod]
        public void SubstringBefore_returns_empty_string_if_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringBefore("."));
        }

        [TestMethod]
        public void SubstringBefore_returns_null_for_null()
        {
            Assert.IsNull(StringUtilities.SubstringBefore(null, "."));
        }

        [TestMethod]
        public void SubstringBeforeLast_finds_part_before_delimiter()
        {
            Assert.AreEqual("1234", singleIndex.SubstringBeforeLast("."));
        }

        [TestMethod]
        public void SubstringBeforeLast_finds_part_before_last_delimiter_with_multiples()
        {
            Assert.AreEqual("1.234.56.234", multipleIndex.SubstringBeforeLast("."));
        }

        [TestMethod]
        public void SubstringBeforeLast_returns_empty_string_if_nothing_before_delimiter()
        {
            Assert.AreEqual("", startIndex.SubstringBeforeLast("."));
        }

        [TestMethod]
        public void SubstringBeforeLast_returns_empty_string_if_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringBeforeLast("."));
        }

        [TestMethod]
        public void SubstringBeforeLast_returns_null_for_null()
        {
            Assert.IsNull(StringUtilities.SubstringBeforeLast(null, "."));
        }
    }
}
