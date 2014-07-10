using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;

namespace TestRoslyn
{
    [TestClass]
    public class StringUtilitiesTests
    {
        private const string SubstringBeforeAferCateogory = "SubstringBeforeAfer";
        private const string ReplaceWithCateogory = "ReplaceWith";

        private string singleIndex = "1234.56";
        private string multipleIndex = "1.234.56.234.56";
        private string endIndex = "123456.";
        private string startIndex = ".123456";
        private string noIndex = "123456";

        #region substring before and after
        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfter_finds_part_after_delimiter()
        {
            Assert.AreEqual("56", singleIndex.SubstringAfter("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfter_finds_part_after_multichar_delimiter()
        {
            Assert.AreEqual("4.56", singleIndex.SubstringAfter("23"));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfter_finds_part_after_first_delimiter_with_multiples()
        {
            Assert.AreEqual("234.56.234.56", multipleIndex.SubstringAfter("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfter_finds_part_after_first_multichar_delimiter_with_multiples()
        {
            Assert.AreEqual("4.56.234.56", multipleIndex.SubstringAfter(".23"));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfter_returns_empty_string_if_nothing_after_delimiter()
        {
            Assert.AreEqual("", endIndex.SubstringAfter("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfter_returns_empty_string_if_nothing_after_multichar_delimiter()
        {
            Assert.AreEqual("", endIndex.SubstringAfter("56."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfter_returns_empty_string_if_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringAfter("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfter_returns_empty_string_if_multichar_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringAfter(".23"));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfter_returns_null_for_null()
        {
            Assert.IsNull(StringUtilities.SubstringAfter(null, "."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        [ExpectedException(typeof(InvalidOperationException ))]
        public void SubstringAfter_throws_on_null_delimiter()
        {
            var x = StringUtilities.SubstringAfter("", null);
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfterLast_finds_part_after_delimiter()
        {
            Assert.AreEqual("56", singleIndex.SubstringAfterLast("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfterLast_finds_part_after_multichar_delimiter()
        {
            Assert.AreEqual("4.56", singleIndex.SubstringAfterLast("23"));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfterLast_finds_part_after_last_delimiter_with_multiples()
        {
            Assert.AreEqual("56", multipleIndex.SubstringAfterLast("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfterLast_finds_part_after_last_multichar_delimiter_with_multiples()
        {
            Assert.AreEqual("4.56", multipleIndex.SubstringAfterLast(".23"));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfterLast_returns_empty_string_if_nothing_after_delimiter()
        {
            Assert.AreEqual("", endIndex.SubstringAfterLast("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfterLast_returns_empty_string_if_nothing_after_multichar_delimiter()
        {
            Assert.AreEqual("", endIndex.SubstringAfterLast("56."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfterLast_returns_empty_string_if_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringAfterLast("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfterLast_returns_empty_string_if_multichar_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringAfterLast(".23"));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringAfterLast_returns_null_for_null()
        {
            Assert.IsNull(StringUtilities.SubstringAfterLast(null, "."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SubstringAfterLast_throws_on_null_delimiter()
        {
            var x = StringUtilities.SubstringAfterLast("", null);
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringBefore_finds_part_before_delimiter()
        {
            Assert.AreEqual("1234", singleIndex.SubstringBefore("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringBefore_finds_part_before_first_delimiter_with_multiples()
        {
            Assert.AreEqual("1", multipleIndex.SubstringBefore("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringBefore_returns_empty_string_if_nothing_before_delimiter()
        {
            Assert.AreEqual("", startIndex.SubstringBefore("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringBefore_returns_empty_string_if_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringBefore("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringBefore_returns_null_for_null()
        {
            Assert.IsNull(StringUtilities.SubstringBefore(null, "."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SubstringBefore_throws_on_null_delimiter()
        {
            var x = StringUtilities.SubstringBefore("", null);
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringBeforeLast_finds_part_before_delimiter()
        {
            Assert.AreEqual("1234", singleIndex.SubstringBeforeLast("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringBeforeLast_finds_part_before_last_delimiter_with_multiples()
        {
            Assert.AreEqual("1.234.56.234", multipleIndex.SubstringBeforeLast("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringBeforeLast_returns_empty_string_if_nothing_before_delimiter()
        {
            Assert.AreEqual("", startIndex.SubstringBeforeLast("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringBeforeLast_returns_empty_string_if_delimiter_not_found()
        {
            Assert.AreEqual("", noIndex.SubstringBeforeLast("."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        public void SubstringBeforeLast_returns_null_for_null()
        {
            Assert.IsNull(StringUtilities.SubstringBeforeLast(null, "."));
        }

        [TestMethod]
        [TestCategory(SubstringBeforeAferCateogory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SubstringBeforeLast_throws_on_null_delimiter()
        {
            var x = StringUtilities.SubstringBeforeLast("", null);
        }

        #endregion

        #region 
        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        public void ReplaceWith_replaces_first_single_single_char()
        {
            Assert.AreEqual("1234-56", singleIndex. ReplaceFirst(".", "-"));
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        public void ReplaceWith_replaces_first_multiple_single_char()
        {
            Assert.AreEqual("1-234.56.234.56", multipleIndex.ReplaceFirst(".", "-"));
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        public void ReplaceWith_replaces_first_single_multiple_char()
        {
            Assert.AreEqual("1-4.56", singleIndex.ReplaceFirst("23", "-"));
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        public void ReplaceWith_replaces_first_multiple_multiple_char()
        {
            Assert.AreEqual("1-4.56.234.56", multipleIndex.ReplaceFirst(".23", "-"));
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        public void ReplaceWith_replaces_all_single_char()
        {
            Assert.AreEqual("-", ".".ReplaceFirst(".", "-"));
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        public void ReplaceWith_replaces_all_multiple_char()
        {
            Assert.AreEqual("-", "23".ReplaceFirst("23", "-"));
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        public void ReplaceWith_is_unchanged_if_string_not_found()
        {
            Assert.AreEqual(multipleIndex, multipleIndex.ReplaceFirst("x", "-"));
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        public void ReplaceWith_rempves_when_replaceWith_is_empty()
        {
            Assert.AreEqual("1.4.56.234.56", multipleIndex.ReplaceFirst("23", ""));
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        public void ReplaceWith_returns_empty_string_if_input_is_empty_string()
        {
            Assert.AreEqual("", "".ReplaceFirst(".", "-"));
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReplaceWith_throws_if_replace_parameter_is_empty_string()
        {
            var x = singleIndex.ReplaceFirst(null, ",");
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        public void ReplaceWith_returns_null_if_input_is_null()
        {
            Assert.IsNull(((string)null).ReplaceFirst(".", ","));
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        [ExpectedException(typeof(InvalidOperationException ))]
        public void ReplaceWith_throws_if_replace_parameter_is_null()
        {
            var x =singleIndex.ReplaceFirst(null, ",");
        }

        [TestMethod]
        [TestCategory(ReplaceWithCateogory)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReplaceWith_throws_if_replace_with_parameter_is_null()
        {
            var x = singleIndex.ReplaceFirst(".", null);
        }


        #endregion

    }
}
