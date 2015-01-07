using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RoslynDom.CSharp
{
   public static class UpdateUtilities
   {
      public static bool DoUpdateOnWorkspace<TChange>(Workspace workspace,
                 Func<IRoot, IEnumerable<TChange>> getItems,
                 IUpdateRefactoring<TChange> updateRefactoring)
              where TChange : IDom
      {
         var solution = workspace.CurrentSolution;
         //var changedDocuments = ChangeAll(solution);
         //foreach(var document in changedDocuments )
         //{
         //   solution = solution.WithDocumentSyntaxRoot(document.Id, document);
         //   workspace.TryApplyChanges(solution);

         //}

         var didAnythingChange = false;
         var newSolution = DoUpdateOnSolution(solution, ref didAnythingChange, getItems, updateRefactoring);
         if (didAnythingChange)
         {
            if (workspace.TryApplyChanges(solution))
            { return true; }
         }
         return didAnythingChange;
      }

      //private static IEnumerable<Document> ChangeAll(Solution solution)
      //{
      //   throw new NotImplementedException();
      //}

      public static Solution DoUpdateOnSolution<TChange>(Solution solution, ref bool didAnythingChange,
            Func<IRoot, IEnumerable<TChange>> getItems,
            IUpdateRefactoring<TChange> updateRefactoring)
         where TChange : IDom
      {
         var projects = solution.Projects.Where(x => x.Language == ExpectedLanguages.CSharp);
         var newSolution = solution;
         foreach (var project in projects)
         {
            var didThisChange = false;
            newSolution = DoUpdateOnProject(project, newSolution, ref didThisChange, getItems, updateRefactoring);
            if (didThisChange)
            { didAnythingChange = true; }
         }

         return newSolution;
      }

      public static Solution DoUpdateOnProject<TChange>(Project project, Solution solution, ref bool didChange,
                  Func<IRoot, IEnumerable<TChange>> getItems,
                  IUpdateRefactoring<TChange> updateRefactoring)
               where TChange : IDom
      {
         var documents = project.Documents.Where(x => x.SupportsSyntaxTree);
         var newSolution = solution;
         foreach (var document in documents)
         {
            var id = document.Id;
            var newSyntaxTree = DoUpdateOnDocument(document, getItems, updateRefactoring);
            if (newSyntaxTree != null)
            {
               didChange = true;
               newSolution = newSolution.WithDocumentSyntaxRoot(id, newSyntaxTree.GetCompilationUnitRoot());
            }
         }
         return newSolution;
      }

      public static SyntaxTree DoUpdateOnDocument<TChange>(Document document,
              Func<IRoot, IEnumerable<TChange>> getItems,
              IUpdateRefactoring<TChange> updateRefactoring)
           where TChange : IDom
      {
         var didThisChange = false;
         SyntaxTree tree;
         if (document.TryGetSyntaxTree(out tree))
         {
            var newTree = UpdateTree(tree, ref didThisChange, getItems, updateRefactoring.NeedsChange, updateRefactoring.MakeChange);
            if (didThisChange)
            {
               return newTree;
            }
         }
         return null;
      }

      public static bool DoUpdateOnFiles<TChange>(IEnumerable<Tuple<string, string>> filePairs,
                 Func<IRoot, IEnumerable<TChange>> getItems,
                 IUpdateRefactoring<TChange> updateRefactoring)
              where TChange : IDom
      {
         return DoUpdateOnFiles(filePairs, getItems, updateRefactoring.NeedsChange, updateRefactoring.MakeChange);
      }

      public static bool DoUpdateOnFiles<TChange>(IEnumerable<Tuple<string, string>> filePairs,
                 Func<IRoot, IEnumerable<TChange>> getItems,
                 Func<TChange, bool> needsChange,
                 Func<TChange, bool> makeChange)
              where TChange : IDom
      {
         var didAnythingChange = false;
         foreach (var pair in filePairs)
         {
            if (DoUpdateOnFile(pair.Item1, pair.Item2,
               getItems, needsChange, makeChange))
            { didAnythingChange = true; }
         }
         return didAnythingChange;
      }

      public static bool DoUpdateOnFile<TChange>(string inputFileName, string outputFileName,
                  Func<IRoot, IEnumerable<TChange>> getItems,
                  Func<TChange, bool> needsChange,
                  Func<TChange, bool> fixItem)
      {
         var code = File.ReadAllText(inputFileName);
         SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
         var didAnythingChange = false;
         var newTree = UpdateTree(tree, ref didAnythingChange, getItems, needsChange, fixItem);
         if (didAnythingChange)
         { WriteToFile(outputFileName, newTree); }
         return didAnythingChange;
      }

      public static SyntaxTree UpdateTree<TChange>(SyntaxTree tree, ref bool didAnythingChange,
                  Func<IRoot, IEnumerable<TChange>> getItems,
                  Func<TChange, bool> needsChange,
                  Func<TChange, bool> fixItem)
      {
         var root = RDom.CSharp.Load(tree);
         var items = getItems(root);
         foreach (var item in items)
         {
            if (needsChange(item))
            {
               if (fixItem(item))
               { didAnythingChange = true; }
            }
         }
         var retTree = RDom.CSharp.GetSyntaxTree(root);
         return retTree;
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

      //public static bool DoUpdateOnFile<TChange>(string inputFileName, string outputFileName,
      //                Func<IRoot, IEnumerable<TChange>> getItemsToChange,
      //                Func<TChange, bool> doChanges)
      //     where TChange : IDom
      //{
      //   var root = RDomCSharp.Factory.GetRootFromFile(inputFileName);
      //   var itemsToChange = getItemsToChange(root);
      //   var didAnythingChange = false;
      //   foreach (var item in itemsToChange)
      //   {
      //      if (doChanges(item))
      //      { didAnythingChange = true; }
      //   }
      //   if (didAnythingChange)
      //   { WriteToFile(outputFileName, root); }
      //   return didAnythingChange;
      //}

      public static void WriteToFile(string outputFileName, IRoot root)
      {
         var output = RDom.CSharp.GetSyntaxNode(root);
         WriteToFile(outputFileName, output.ToFullString());
      }

      public static void WriteToFile(string outputFileName, SyntaxNode node)
      {
         WriteToFile(outputFileName, node.ToFullString());
      }

      public static void WriteToFile(string outputFileName, SyntaxTree tree)
      {
         WriteToFile(outputFileName, tree.GetCompilationUnitRoot().ToFullString());
      }

      private static void WriteToFile(string outputFileName, string output)
      {
         var sb = new StringBuilder();
         sb.Append(output);
         var dirName = Path.GetDirectoryName(outputFileName);
         if (!string.IsNullOrWhiteSpace(dirName)) Directory.CreateDirectory(dirName);
         File.WriteAllText(outputFileName, sb.ToString());
      }
   }
}