using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using RoslynDom.Common;
using System.Linq;

namespace TestRoslyn
{
    [TestClass]
    public class FileSupportTests
    {
        [TestMethod]
        public void ProjectPath_finds_project_in_current_directory()
        {
            // This test expects current file structure
            var path = Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory));
            Assert.IsNotNull(FileSupport.ProjectPath(path));
        }

        [TestMethod]
        public void ProjectPath_finds_project_in_parent_directory()
        {
            // This test expects current file structure
            var path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            Assert.IsNotNull(FileSupport.ProjectPath(path));
        }

        [TestMethod]
        public void ProjectPath_finds_project_in_higher_directory()
        {
            // This test expects current file structure
            var path = AppDomain.CurrentDomain.BaseDirectory;
            Assert.IsNotNull(FileSupport.ProjectPath(path));
        }

        [TestMethod]
        public void ProjectPath_returns_null_if_no_project_in_directory_hierarchy()
        {
            // This test is vulnerable to issues with directory structure, specifically, an extra csproj or vbproj file can lead to test failure
            var path = Path.GetDirectoryName(
                        Path.GetDirectoryName(
                        Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
            Assert.IsNull(FileSupport.ProjectPath(path));
        }

        [TestMethod]
        public void GetMatchingFiles_finds_files()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            Assert.AreEqual(3, FileSupport.GetMatchingFiles("*.kadcs", FileSupport.ProjectPath(path), true).Count());
            Assert.AreEqual(1, FileSupport.GetMatchingFiles("*.kadcs", FileSupport.ProjectPath(path), false).Count());
            var pattern = Path.Combine("..\\..", "*.kadcs");
            Assert.AreEqual(3, FileSupport.GetMatchingFiles("*.kadcs", FileSupport.ProjectPath(path), true).Count());
            Assert.AreEqual(1, FileSupport.GetMatchingFiles("*.kadcs", FileSupport.ProjectPath(path), false).Count());
            Assert.AreEqual(1, FileSupport.GetMatchingFiles("*.kadcs", FileSupport.ProjectPath(path)).Count());
        }

        [TestMethod]
        public void GetMatchingFiles_doesnt_crash_on_no_files()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            Assert.AreEqual(0, FileSupport.GetMatchingFiles("*.xadman", FileSupport.ProjectPath(path), true).Count());
            Assert.AreEqual(0, FileSupport.GetMatchingFiles("*.xadman", FileSupport.ProjectPath(path), false).Count());
        }

        [TestMethod]
        public void GetFileContents_returns_null_if_no_file()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var file = Path.Combine(path, "normal.kadcx");
            var results = FileSupport.GetFileContents(file);
            Assert.IsNull(results);
        }

        [TestMethod]
        public void GetFileContents_returns_contents()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var file = Path.Combine(path, @"..\..\normal.kadcs");
            var results = FileSupport.GetFileContents(file);
            Assert.IsTrue(results.StartsWith("using KadMan.Common;"));
        }

      [TestMethod]
      public void GetNearestFilesOfType_adds_period_when_needed()
      {
         var path = AppDomain.CurrentDomain.BaseDirectory;
         var files = FileSupport.GetNearestFilesOfType(path, "dll");
         Assert.IsTrue(files.Count() > 0);
      }

      [TestMethod]
      public void GetNearestSolution_finds_existing_solution()
      {
         var path = AppDomain.CurrentDomain.BaseDirectory;
         var file = FileSupport.GetNearestSolution(path);
         Assert.IsTrue(file.EndsWith(@"\RoslynDom.sln"));
      }

      [TestMethod]
      public void GetNearestSolution_returns_null_for_no_solution()
      {
         var path = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\..\..";
         var file = FileSupport.GetNearestSolution(path);
         Assert.IsNull(file);
      }

      [TestMethod]
      public void GetNearestSolution_finds_solution_from_filename()
      {
         var path = AppDomain.CurrentDomain.BaseDirectory + @"\UtilitiesTests.dll";
         var file = FileSupport.GetNearestSolution(path);
         Assert.IsTrue(file.EndsWith(@"\RoslynDom.sln"));
      }

      [TestMethod]
      public void GetNearestSolution_finds_solution_from_solution_path()
      {
         var path = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\RoslynDom.sln";
         var file = FileSupport.GetNearestSolution(path);
         Assert.IsTrue(file.EndsWith(@"\RoslynDom.sln"));
      }

      [TestMethod]
      public void GetNearestCSharpProject_finds_existing_project()
      {
         var path = AppDomain.CurrentDomain.BaseDirectory;
         var file = FileSupport.GetNearestCSharpProject(path);
         Assert.IsTrue(file.EndsWith(@"\UtilitiesTests.csproj"));
      }

      [TestMethod]
      public void GetNearestCSharpProject_returns_null_for_no_project()
      {
         var path = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\..\..";
         var file = FileSupport.GetNearestCSharpProject(path);
         Assert.IsNull(file);
      }

      [TestMethod]
      public void GetNearestCSharpProject_finds_project_from_filename()
      {
         var path = AppDomain.CurrentDomain.BaseDirectory + @"\UtilitiesTests.dll";
         var file = FileSupport.GetNearestCSharpProject(path);
         Assert.IsTrue(file.EndsWith(@"\UtilitiesTests.csproj"));
      }

      [TestMethod]
      public void GetNearestCSharpProject_finds_project_from_project_path()
      {
         var path = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\UtilitiesTests.csproj";
         var file = FileSupport.GetNearestCSharpProject(path);
         Assert.IsTrue(file.EndsWith(@"\UtilitiesTests.csproj"));
      }


   }
}
