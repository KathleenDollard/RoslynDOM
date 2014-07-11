using System;
using System.IO;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using RoslynDom;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;

namespace RoslynDomExampleTests
{
    [TestClass]
    public class RoslynDomLoad
    {
        [TestMethod]
        public void Load_root_from_file_example()
        {
            IRoot root = RDomFactory.GetRootFromFile(@"..\..\TestFile.cs");
            Assert.IsNotNull(root);
            Assert.AreEqual(3, root.Namespaces.Count());
        }

        [TestMethod]
        public void Load_root_from_string_example()
        {
            var csharpCode = File.ReadAllText(@"..\..\TestFile.cs");
            IRoot root = RDomFactory.GetRootFromString(csharpCode);
            Assert.IsNotNull(root);
            Assert.AreEqual(3, root.Namespaces.Count());
        }

        [TestMethod]
        public void Load_root_from_syntaxtree_example()
        {
            var csharpCode = File.ReadAllText(@"..\..\TestFile.cs");
            var tree = CSharpSyntaxTree.ParseText(csharpCode);
            var root = RDomFactory.GetRootFromSyntaxTree(tree);
            Assert.IsNotNull(root);
            Assert.AreEqual(3, root.Namespaces.Count());
        }

        [TestMethod]
        public void Load_root_from_document_example()
        {
            var slnFile = TestUtilities.GetNearestSolution(Environment.CurrentDirectory);
            var ws = MSBuildWorkspace.Create();
            var solution = ws.OpenSolutionAsync(slnFile).Result;
            var project = solution.Projects.Where(x => x.Name == "RoslynDomExampleTests").FirstOrDefault();
            var document = project.Documents.Where(x => x.Name == "TestFile.cs").FirstOrDefault();
            Assert.IsNotNull(document);
            var root = RDomFactory.GetRootFromDocument(document);
            Assert.IsNotNull(root);
            Assert.AreEqual(3, root.Namespaces.Count());
        }


    }
}
