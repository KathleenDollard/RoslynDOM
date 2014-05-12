using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using KadGen.Common;
using System.Linq;

namespace TestRoslyn
{
    [TestClass]
    public class TestFileSupport
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

    }
}
