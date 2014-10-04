using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;
using RoslynDom.CSharp;
using RoslynDom;
using Microsoft.CodeAnalysis.Formatting;

namespace RoslynDomExampleTests
{
   [TestClass]
   public class Walkthroughs_2_INotifyPropertyChanged
   {
      private string inputDirectory = @"..\..\..\RoslynDom";
      private string outputDirectory = @"..\..\Walkthrough2_Updated_Files";

      [TestMethod]
      public void Walkthroughs_2_load_files()
      {
         UpdateFilesInDirectory(inputDirectory, outputDirectory, @"BasesAndBaseHelpers");
         UpdateFilesInDirectory(inputDirectory, outputDirectory, @"Implementations");
         UpdateFilesInDirectory(inputDirectory, outputDirectory, @"StatementImplementations");
      }

      [TestMethod]
      public void Walkthroughs_2_update_files()
      {
         IEnumerable<Tuple<string, string>> filePairs = GetFilePairs();
         foreach (var pair in filePairs)
         {
            DoUpdate(pair.Item1, pair.Item2, NotifyPropertyChange);
         }
      }

      private bool DoUpdate(string inputFileName, string outputFileName, Func<IRoot, bool> doChanges)
      {
         var factory = RDomCSharp.Factory;
         var root = factory.GetRootFromFile(inputFileName);
         if (doChanges(root))
         {
            var output = factory.BuildSyntax(root);
            var outputString = output.ToFullString();
            File.WriteAllText(outputFileName, outputString);
            return true;
         }
         else { return false; }
      }

      private bool NotifyPropertyChange(IRoot root)
      {
         IEnumerable<IClass> classes = GetClasses(root);
         if (!classes.Any()) return false;
         foreach (var cl in classes)
         { AddINotifyPropertyChanged(cl); }
         return true;
      }

      private IEnumerable<Tuple<string, string>> GetFilePairs()
      {
         var subDirectories = new string[] { @"BasesAndBaseHelpers", @"Implementations", @"StatementImplementations" };
         IEnumerable<Tuple<string, string>> filePairs = new List<Tuple<string, string>>();
         foreach (var subDir in subDirectories)
         {
            filePairs = filePairs.Union(
               Directory
                  .GetFiles(Path.Combine(inputDirectory, subDir), "*.cs")
                  .Select(x => Tuple.Create(x, Path.Combine(outputDirectory, subDir, Path.GetFileName(x)))));
         }

         return filePairs;
      }

      [TestMethod]
      public void Walkthroughs_2_list_files()
      {
         var sb = new StringBuilder();
         ListFilesInDirectory(inputDirectory, @"BasesAndBaseHelpers", sb);
         ListFilesInDirectory(inputDirectory, @"Implementations", sb);
         ListFilesInDirectory(inputDirectory, @"StatementImplementations", sb);
      }

      private void ListFilesInDirectory(string inputDirectory, string subDirectory, StringBuilder sb)
      {
         var factory = RDomCSharp.Factory;
         var inputDir = Path.Combine(inputDirectory, subDirectory);
         var files = Directory.GetFiles(inputDir, "*.cs");
         foreach (var fileName in files)
         {
            var root = factory.GetRootFromFile(fileName);
            IEnumerable<IClass> classes = GetClasses(root);
            foreach (var cl in classes)
            { sb.AppendLine(cl.Name); }
         }
      }


      [TestMethod]
      public void Walkthroughs_2_load_specific_file()
      {
         var factory = RDomCSharp.Factory;
         UpdateFile(factory, outputDirectory, Path.Combine(inputDirectory, @"StatementImplementations", "RDomInvocationStatement.cs"));
      }

      private void UpdateFilesInDirectory(string inputDirectory, string outputDirectory, string subDirectory)
      {
         var factory = RDomCSharp.Factory;
         var inputDir = Path.Combine(inputDirectory, subDirectory);
         var outputDir = Path.Combine(outputDirectory, subDirectory);
         var files = Directory.GetFiles(inputDir, "*.cs");
         foreach (var fileName in files)
         {
            UpdateFile(factory, outputDir, fileName);
         }
      }

      private void UpdateFile(RDomCSharp factory, string outputDir, string fileName)
      {
         var outputFileName = Path.Combine(outputDir, Path.GetFileName(fileName));
         var root = factory.GetRootFromFile(fileName);
         IEnumerable<IClass> classes = GetClasses(root);
         foreach (var cl in classes)
         { AddINotifyPropertyChanged(cl); }
         var output = factory.BuildSyntax(root);
         //output = factory.Format(output,);
         var outputString = output.ToFullString();
         File.WriteAllText(outputFileName, outputString);
      }

      private static IEnumerable<IClass> GetClasses(IRoot root)
      {
         return root.RootClasses
                        .Where(x => x.BaseType != null
            && ((x.BaseType.Name == "RDomBaseType" && x.BaseType.TypeArguments.Count() == 1)
                || (x.BaseType.Name.StartsWith("RDomBase") && x.BaseType.TypeArguments.Count() == 2)));
      }

      private void AddINotifyPropertyChanged(IClass cl)
      {
         var notifyingProps = cl.Properties
                              .Where(x => x.CanSet
                                    && !x.SetAccessor.Statements.Any()
                                    && !x.GetAccessor.Statements.Any()
                                    && x.AccessModifier == AccessModifier.Public);
         foreach (var prop in notifyingProps)
         {
            UpdateProperty(prop);
         }
      }

      private void UpdateProperty(IProperty prop)
      {
         // Add the field without further checks because the programmer will find and resolve
         // things like naming collisions
         var parent = prop.Parent as ITypeMemberContainer;
         var fieldName = StringUtilities.CamelCase(prop.Name);
         var field = new RDomField(fieldName, prop.ReturnType, declaredAccessModifier: AccessModifier.Private);
         FixWhitespace(field, prop);
         field.Whitespace2Set.Add(new Whitespace2(prop.Whitespace2Set.First().Copy()));
         parent.MembersAll.InsertOrMoveBefore(prop, field);
         UpdatePropertyGet(prop, fieldName);
         UpdatePropertySet(prop, fieldName);
      }

      private void FixWhitespace(RDomField field, IProperty prop)
      {
         // TODO: This is rather detailed because of featuresnot yet in the whitespace system
         var leading = prop.Whitespace2Set[LanguageElement.Public].LeadingWhitespace;
         field.Whitespace2Set[LanguageElement.Private] = new Whitespace2(LanguageElement.Private, leading, " ", "");
      }

      private void UpdatePropertyGet(IProperty prop, string fieldName)
      {
         var retExpression = RDomCSharp.Factory.ParseExpression(fieldName);
         var statement = new RDomReturnStatement(retExpression, true);
         prop.GetAccessor.StatementsAll.AddOrMove(statement);
         prop.GetAccessor.EnsureNewLineAfter();
      }

      private void UpdatePropertySet(IProperty prop, string fieldName)
      {
         var expression = RDomCSharp.Factory.ParseExpression(string.Format("SetProperty(ref {0}, value)", fieldName));
         var statement = new RDomInvocationStatement(expression, true);
         prop.SetAccessor.StatementsAll.AddOrMove(statement);
         prop.GetAccessor.EnsureNewLineAfter();
      }

      private string ReportCodeLines(IEnumerable<IDom> items)
      {
         var sb = new StringBuilder();

         var lineItems = from x in items
                         select new
                         {
                            item = x,
                            fileName = GetFileName(x),
                            position = GetPosition(x),
                            code = GetNewCode(x)
                         };
         var filePathMax = lineItems.Max(x => x.fileName.Length);
         var itemMax = lineItems.Max(x => x.item.ToString().Trim().Length);
         var lineMax = lineItems.Max(x => x.position.Line.ToString().Trim().Length);
         var format = "{0, -fMax}({1,lineMax},{2,3}) {3, -itemMax}   {4}"
                     .Replace("fMax", filePathMax.ToString())
                     .Replace("itemMax", itemMax.ToString())
                     .Replace("lineMax", lineMax.ToString());
         foreach (var line in lineItems)
         {
            sb.AppendFormat(format, line.fileName, line.position.Line, line.position.Character, line.item.ToString().Trim(), line.code);
            sb.AppendLine();
         }
         return sb.ToString();
      }

      private string GetNewCode(IDom item)
      {
         var ret = new List<string>();
         // formatting removed from the following line (BuildFormattedSyntax)
         return RDomCSharp.Factory.BuildSyntax(item).ToString();
         //   return RDomCSharp.Factory.BuildFormattedSyntax(item).ToString();
      }

      private string GetOldCode(IDom item)
      {

         var node = item.RawItem as SyntaxNode;
         if (node == null)
         { return "<no syntax node>"; }
         else
         {
            return node.ToFullString();
         }
      }

      private LinePosition GetPosition(IDom item)
      {
         var node = item.RawItem as SyntaxNode;
         if (node == null)
         { return default(LinePosition); }
         else
         {
            var location = node.GetLocation();
            var linePos = location.GetLineSpan().StartLinePosition;
            return linePos;
         }
      }

      private string GetFileName(IDom item)
      {
         var root = item.Ancestors.OfType<IRoot>().FirstOrDefault();
         if (root != null)
         { return root.FilePath; }
         else
         {
            var top = item.Ancestors.Last();
            var node = top as SyntaxNode;
            if (node == null)
            { return "<no file name>"; }
            else
            { return node.SyntaxTree.FilePath; }
         }
      }
   }
}
