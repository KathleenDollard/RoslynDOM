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

namespace RoslynDomExampleTests
{
   [TestClass]
   public class Walkthroughs_2_INotifyPropertyChanged
   {
      private string inputDirectory = @"..\..\..\RoslynDom";
      private string outputDirectory = @"..\..\Walkthrough2_Updated_Files";

      [TestMethod]
      public void Walkthroughs_2_LoadFiles()
      {
         var factory = RDomCSharp.Factory;
         var files = Directory.GetFiles(inputDirectory, "*.cs");
         foreach (var fileName in files)
         {
            var outputFileName = Path.Combine(outputDirectory, Path.GetFileName(fileName));
            var root = factory.GetRootFromFile(fileName);
            var classes = root.RootClasses;
            foreach(var cl in classes)
            { AddINotifyPropertyChanged(cl); }
            var output = factory.BuildSyntax(root).ToString();
            File.WriteAllText(outputFileName, output);
         }
      }

      private void AddINotifyPropertyChanged(IClass cl)
      {
         //var interfaces = ;
         //if (!cl.ImplementedInterfaces.Any(x=>x.Name=="INotifyPropertyChanged"))
         //   { AddInterface(cl, "INotifyPropertyChanged"); }
         //if (!cl.Events.Any(x=>x.Name == "PropertyChangedEvent"))
         //{ AddEvent(cl, "PropertyChangedEvent"); }
         //var notifyingProps = cl.Properties
         //                     .Where(x => x.CanSet);
         //foreach (var prop in notifyingProps)
         //{
         //   UpdateProperty(prop); 
         //}

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
         return RDomCSharp.Factory.BuildFormattedSyntax(item).ToString();
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
