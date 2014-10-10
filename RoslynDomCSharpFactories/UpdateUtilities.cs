using RoslynDom.Common;
using RoslynDom.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.CSharp
{
   public static class UpdateUtilities
   {
      public static bool DoUpdate<TChange>(IEnumerable<Tuple<string, string>> filePairs,
                 Func<IRoot, IEnumerable<TChange>> getItems,
                 Func<TChange, bool> needsChange,
                 Func<TChange, bool> fixItem)
              where TChange : IDom
      {
         var didAnythingChange = false;
         foreach (var pair in filePairs)
         {
            if (DoUpdateOnFile(pair.Item1, pair.Item2,
               getItems, needsChange, fixItem))
               ;
            { didAnythingChange = true; }
         }
         return didAnythingChange;
      }


      public static bool DoUpdateOnFile<TChange>(string inputFileName, string outputFileName,
                  Func<IRoot, IEnumerable<TChange>> getItems,
                  Func<TChange, bool> needsChange,
                  Func<TChange, bool> fixItem)
      {
         var root = RDomCSharp.Factory.GetRootFromFile(inputFileName);
         var items = getItems(root);
         var didAnythingChange = false;
         foreach (var item in items)
         {
            if (needsChange(item))
            {
               if (fixItem(item))
               { didAnythingChange = true; }
            }
         }
         if (didAnythingChange)
         { WriteRootToFile(outputFileName, root); }
         return didAnythingChange;

      }

      public static IEnumerable<Tuple<string, string>> GetFilePairs(string pattern,
                string inputDirectory, string outputDirectory, params string[] subDirectories)
      {
         IEnumerable<Tuple<string, string>> filePairs = new List<Tuple<string, string>>();
         foreach (var subDir in subDirectories)
         {
            filePairs = filePairs.Union(
               Directory
                  .GetFiles(Path.Combine(inputDirectory, subDir), pattern)
                  .Select(x => Tuple.Create(x, Path.Combine(outputDirectory, subDir, Path.GetFileName(x)))));
         }

         return filePairs;
      }

      public static bool DoUpdateOnFile<TChange>(string inputFileName, string outputFileName,
                      Func<IRoot, IEnumerable<TChange>> getItemsToChange,
                      Func<TChange, bool> doChanges)
           where TChange : IDom
      {
         var root = RDomCSharp.Factory.GetRootFromFile(inputFileName);
         var itemsToChange = getItemsToChange(root);
         var didAnythingChange = false;
         foreach (var item in itemsToChange)
         {
            if (doChanges(item))
            { didAnythingChange = true; }
         }
         if (didAnythingChange)
         { WriteRootToFile(outputFileName, root); }
         return didAnythingChange;
      }

      public static void WriteRootToFile(string outputFileName, IRoot root)
      {
         var sb = new StringBuilder();
         var output = RDomCSharp.Factory.BuildSyntax(root);
         sb.Append(output.ToFullString());
         var dirName = Path.GetDirectoryName(outputFileName);
         Directory.CreateDirectory(dirName);
         File.WriteAllText(outputFileName, sb.ToString());
      }

     
   }
}
