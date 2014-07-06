using System;
using System.IO;
using System.Linq;

namespace RoslynDomTests
{
    internal static class TestUtilities
    {
        internal  static string GetNearestSolution(string path)
        {
            if (Path.GetExtension(path) == ".sln") return path;
            string currentDirectory = path;
            var slnFile = "";
            while (string.IsNullOrWhiteSpace(slnFile))
            {
                var files = Directory.GetFiles(currentDirectory, "*.sln");
                var count = files.Count();
                if (count > 1) throw new InvalidOperationException("Ambiguous solution files discovered");
                if (count == 1)
                { slnFile = files.First(); }
                else
                {
                    // This is a quick and dirty sample, so use exceptions this way in a real app
                    try
                    { currentDirectory = Path.GetDirectoryName(currentDirectory); }
                    catch
                    { return null; }
                }
            }
            return slnFile;
        }
    }
}
