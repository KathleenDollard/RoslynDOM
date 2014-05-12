using KadGen.Common.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace TestRoslyn
{
    [TestClass]
    public class TestRoslynSyntaxElementNamespace : TestRoslynSyntaxElement<NamespaceDeclarationSyntax>
    {
        public TestRoslynSyntaxElementNamespace() : base(typeof(RoslynSyntaxSupportNamespace), "Namespace")
        {
            totalFlattenedCountRoot = 5;
            totalNotFlattenedCountRoot = 1;
            totalFlattenedCountNamespaceB = 3;
            totalNotFlattenedCountNamespaceB = 2;
        }

//        private string treeForNamespaceTests = @"
//namespace ETWPreMan
//{
//    namespace B
//    {
//        public class NormalEventSource
//        {
//            [Version(2)]
//            [Keyword()]
//            public void foo2(int Bar, string Bar2)      {        }
//        }
//        namespace B
//        {
//        }
//    }
//}
//";

//        // This pattern shoudl exist in the following groups (other tests might be included)
//        //   - usings
//        //   - namespaces
//        //   - usings
//        //   - interfaces
//        //   - classes
//        // might also later be added to methods, parameters, properties, etc.

//        [TestMethod]
//        public void GetNamespaces_correct_flattened()
//        {
//            SyntaxTree tree = CSharpSyntaxTree.ParseText(treeForNamespaceTests);
//            Assert.AreEqual(3, RoslynSupport.GetNamespaces(tree).Count());
//        }

//        [TestMethod]
//        public void GetNamespaces_correct_not_flattened()
//        {
//            SyntaxTree tree = CSharpSyntaxTree.ParseText(treeForNamespaceTests);
//            Assert.AreEqual(1, RoslynSupport.GetNamespaces(tree, true).Count());
//        }

//        [TestMethod]
//        public void GetNamespaces_correct_arbitrary_location()
//        {
//            SyntaxTree tree = CSharpSyntaxTree.ParseText(treeForNamespaceTests);
//            var root = tree.GetRootOrRootNamespace();
//            Assert.AreEqual(2, RoslynSupport.GetNamespaces(root).Count());
//        }

//        [TestMethod]
//        public void GetNamespaceByName_from_root()
//        {
//            SyntaxTree tree = CSharpSyntaxTree.ParseText( RoslynTestCommon.treeForClassTests);
//            var nspace = tree.GetNamespaceByName("B");
//            Assert.IsNotNull(nspace);
//        }

//        [TestMethod]
//        public void GetNamespaceByName_arbitrary_location()
//        {
//            SyntaxTree tree = CSharpSyntaxTree.ParseText(RoslynTestCommon.treeForClassTests);
//            var root = tree.GetNamespaceByName("ETWPreMan");
//            var nspace = tree.GetNamespaceByName("B");
//            Assert.IsNotNull(nspace);
//        }

//        [TestMethod]
//        public void GetNamespaceByName_returns_null_when_not_found()
//        {
//            SyntaxTree tree = CSharpSyntaxTree.ParseText(RoslynTestCommon.treeForClassTests);
//            var nspace = tree.GetNamespaceByName("XB");
//            Assert.IsNull(nspace);
//        }


//        [TestMethod]
//        public void GetNamespaceByName_throws_exception_if_multiple()
//        {
//            SyntaxTree tree = CSharpSyntaxTree.ParseText(RoslynTestCommon.treeForClassTests);
//            var nspace = tree.GetNamespaceByName("XB");
//            Assert.IsNull(nspace);
//        }

//        [TestMethod]
//        public void GetNamespaceByName_respects_do_not_flatten_attribute()
//        {
//            SyntaxTree tree = CSharpSyntaxTree.ParseText(RoslynTestCommon.treeForClassTests);
//            var nspace = tree.GetNamespaceByName("XB");
//            Assert.IsNull(nspace);
//        }
    }
}