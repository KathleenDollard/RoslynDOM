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
    public class CopyTests
    {
        private const string SameIntentTestCategory = "SameIntent";
        private const string CopyTestCategory = "CopyTests";

        #region clone tests
        [TestMethod,TestCategory(CopyTestCategory)]
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

        [TestMethod,TestCategory(CopyTestCategory)]
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

        [TestMethod, TestCategory(CopyTestCategory)]
        public void Can_clone_namespace()
        {
            Assert.Inconclusive();
        }

        [TestMethod, TestCategory(CopyTestCategory)]
        public void Can_clone_class()
        {
            Assert.Inconclusive();
        }
        #endregion
    }
}
