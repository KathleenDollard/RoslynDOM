using KadGen.Common.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace TestRoslyn
{
    [TestClass]
    public class TestRoslynSyntaxElementUsing : TestRoslynSyntaxElement<UsingDirectiveSyntax>
    {
        public TestRoslynSyntaxElementUsing() : base(typeof(RoslynSyntaxSupportUsing), "Using")
        {
            totalFlattenedCountRoot = 6;
            totalNotFlattenedCountRoot = 2;
            totalFlattenedCountNamespaceB = 3;
            totalNotFlattenedCountNamespaceB = 2;
        }
        //        private string treeForUsingTests = @"
        //using A;
        //using B;
        //namespace ETWPreMan
        //{
        //    using C;
        //    public class NormalEventSource
        //    {
        //        using D;

        //        [Version(2)]
        //        [Keyword()]
        //        public void foo2(int Bar, string Bar2)      {        }
        //    }
        //}
        //";
        //        [TestMethod]
        //        public void GetUsings_correct_flattened()
        //        {
        //            SyntaxTree tree = CSharpSyntaxTree.ParseText(treeForUsingTests);
        //            Assert.AreEqual(3, RoslynSupport.GetUsings(tree).Count());
        //        }

        //        [TestMethod]
        //        public void GetUsings_correct_not_flattened()
        //        {
        //            SyntaxTree tree = CSharpSyntaxTree.ParseText(treeForUsingTests);
        //            Assert.AreEqual(2, RoslynSupport.GetUsings(tree, true).Count());
        //        }

        //        [TestMethod]
        //        public void GetUsings_correct_arbitrary_location()
        //        {
        //            SyntaxTree tree = CSharpSyntaxTree.ParseText(treeForUsingTests);
        //            var root = tree.GetRootOrRootNamespace();
        //            Assert.AreEqual(1, RoslynSupport.GetUsings(root).Count());
        //        }
    }
}