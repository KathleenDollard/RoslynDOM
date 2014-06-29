using System;
using System.IO;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace RoslynDomExampleTests
{
    /// <summary>
    /// This gives examples of accessing the Roslyn workspaces. RoslynDom does not
    /// currently offer workspace features. Request any features you want. 
    /// </summary>
    [TestClass]
    public class WorkspaceTests
    {
        [TestMethod]
        public void Open_solution()
        {
            // Find the solution directory in this root. 
            // this test will break if there are multiple solutions
            var slnFile = TestUtilities.GetNearestSolution(Environment.CurrentDirectory);

            var ws = MSBuildWorkspace.Create();
            // For now: wait for the result
            var solution = ws.OpenSolutionAsync(slnFile).Result;
            // I don't want to test much about the solution - because things like project count will change
            Assert.IsNotNull(solution);
        }

        [TestMethod]
        public void Open_project()
        {
            var slnFile = TestUtilities.GetNearestSolution(Environment.CurrentDirectory);

            var ws = MSBuildWorkspace.Create();
            // For now: wait for the result
            var solution = ws.OpenSolutionAsync(slnFile).Result;
            var project = solution.Projects.Where(x => x.Name == "RoslynDomExampleTests").FirstOrDefault();
            // I don't want to test much about the project - because things will change
            Assert.IsNotNull(project);
        }

        [TestMethod]
        public void Open_document()
        {
            var slnFile = TestUtilities.GetNearestSolution(Environment.CurrentDirectory);

            var ws = MSBuildWorkspace.Create();
            // For now: wait for the result
            var solution = ws.OpenSolutionAsync(slnFile).Result;
            var project = solution.Projects.Where(x => x.Name == "RoslynDomExampleTests").FirstOrDefault();
            var document = project.Documents.Where(x => x.Name == "WorkspaceTests.cs").FirstOrDefault();
            // I don't want to test much about the document - because things will change
            Assert.IsNotNull(document);
        }


    }
}
