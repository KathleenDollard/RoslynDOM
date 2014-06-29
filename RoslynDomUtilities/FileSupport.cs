using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RoslynDom.Common
{
    public static class FileSupport
    {
        public static IEnumerable<String> GetMatchingFiles(
                string pattern,
                string startDirectory,
                bool includeSubdirectories = false)
        {

            var options = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return Directory.GetFiles(startDirectory, pattern, options);
        }

        public static string ProjectPath(string path)
        {
            if (Path.GetPathRoot(path) == path) return null;
            var files = Directory.GetFiles(path, "*.csproj", SearchOption.TopDirectoryOnly)
                        .Union(Directory.GetFiles(path, "*.vbproj", SearchOption.TopDirectoryOnly));
            if (files.Any()) return path;
            return ProjectPath(Path.GetDirectoryName(path));
        }

        public static string GetFileContents(string path)
        {
            // I want to isolate this for fear I've got trouble with unicode as I localize
            // and because I want not finding the file to return null
            if (File.Exists(path)) return File.ReadAllText(path);
            return null;
        }
    }
}
