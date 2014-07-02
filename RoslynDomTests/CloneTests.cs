using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;

namespace RoslynDomTests
{
    [TestClass]
    public class CloneTests
    {
        private const string SameIntentTestCategory = "SameIntent";
        private const string CloneTestCategory = "CloneTests";

        #region clone tests
        [TestMethod,TestCategory(CloneTestCategory)]
        public void Can_clone_attribute()
        {
            Assert.Inconclusive();
            var csharpCode = @"
            using System;
            ";
            var root = RDomFactory.GetRootFromString(csharpCode);
            var newRoot = root.Copy();
            Assert.IsNotNull(newRoot);

        }

        [TestMethod,TestCategory(CloneTestCategory)]
        public void Can_clone_root()
        {
            var csharpCode = @"
            using System;
            ";
            Assert.Inconclusive();
            var root = RDomFactory.GetRootFromString(csharpCode);
            var newRoot = root.Copy();
            Assert.IsNotNull(newRoot);

        }

        [TestMethod, TestCategory(CloneTestCategory)]
        public void Can_clone_namespace()
        {
            Assert.Inconclusive();
        }

        [TestMethod, TestCategory(CloneTestCategory)]
        public void Can_clone_class()
        {
            Assert.Inconclusive();
        }
        #endregion
    }
}
