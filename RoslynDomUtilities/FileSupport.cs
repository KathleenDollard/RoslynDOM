using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RoslynDom.Common
{
   public static class FileSupport
   {
      public static IEnumerable<String> GetMatchingFiles(
             string pattern,
             string startDirectory)
      { return GetMatchingFiles(pattern, startDirectory, false); }

      public static IEnumerable<String> GetMatchingFiles(
              string pattern,
              string startDirectory,
              bool includeSubdirectories)
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

      public static string GetNearestSolution(string path)
      {
         var files = GetNearestFilesOfType(path, ".sln");
         var count = files.Count();
         if (count > 1) throw new InvalidOperationException("Ambiguous solution files discovered");
         return files.FirstOrDefault();
      }

      public static string GetNearestCSharpProject(string path)
      {
         var files = GetNearestFilesOfType(path, ".csproj");
         var count = files.Count();
         if (count > 1) throw new InvalidOperationException("Ambiguous project files discovered");
         return files.FirstOrDefault();
      }

      private static IEnumerable<string> GetNearestFilesOfType(string path, string extension)
      {
         if (!extension.StartsWith("*")) { extension = "*" + extension; }
         if (File.Exists(path)) { path = Path.GetDirectoryName(path); }
         if (Path.GetExtension(path) == extension) return new[] { path };
         string currentDirectory = path;
         while (!string.IsNullOrWhiteSpace(currentDirectory))
         {
            var files = Directory.GetFiles(currentDirectory, extension);
            if (files.Any()) { return files; }
            currentDirectory = Path.GetDirectoryName(currentDirectory);
         }
         return new string[] { };
      }
   }
}
