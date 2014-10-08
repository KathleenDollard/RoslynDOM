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
      private string[] subDirectories = new string[] { @"BasesAndBaseHelpers", @"Implementations", @"StatementImplementations" };

      [TestMethod]
      public void Walkthroughs_1_update_using_for_data_annotation()
      {
         DoUpdate(GetFilePairs("*.cs", inputDirectory, outputDirectory, subDirectories),
            GetRoot, AddUsingStatement("System.ComponentModel.DataAnnotations"));
      }

      [TestMethod]
      public void Walkthroughs_2_update_files()
      {
         DoUpdate(GetFilePairs(".cs", inputDirectory, outputDirectory, subDirectories),
            GetRDomClasses, AddINotifyPropertyChanged);
      }

      private bool DoUpdate<TChange>(IEnumerable<Tuple<string, string>> filePairs,
                            Func<IRoot, IEnumerable<TChange>> getItemsToChange,
                            Func<TChange, bool> changeItems)
         where TChange : IDom
      {
         var didAnythingChange = false;
         foreach (var pair in filePairs)
         {
            if (DoUpdateOnFile(pair.Item1, pair.Item2, getItemsToChange, changeItems))
            { didAnythingChange = true; }
         }
         return didAnythingChange;
      }

      private bool DoUpdateOnFile<TChange>(string inputFileName, string outputFileName,
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
         { WriteChangesToFile(outputFileName, itemsToChange); }
         return didAnythingChange;
      }

      private static void WriteChangesToFile<TChange>(string outputFileName, IEnumerable<TChange> itemsToChange)
         where TChange : IDom
      {
         var sb = new StringBuilder();
         foreach (var item in itemsToChange)
         {
            var output = RDomCSharp.Factory.BuildSyntax(item);
            sb.Append(output.ToFullString());
         }
         File.WriteAllText( outputFileName, sb.ToString());
      }


      private IEnumerable<Tuple<string, string>> GetFilePairs(string pattern,
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

      private static IEnumerable<IRoot> GetRoot(IRoot root)
      {
         return new List<IRoot>() { root };
      }

      private static IEnumerable<IClass> GetRDomClasses(IRoot root)
      {
         return root.RootClasses
                        .Where(x => x.BaseType != null
            && ((x.BaseType.Name == "RDomBaseType" && x.BaseType.TypeArguments.Count() == 1)
                  || (x.BaseType.Name.StartsWith("RDomBase") && x.BaseType.TypeArguments.Count() == 2)));
      }

      private Func<IRoot, bool> AddUsingStatement(params string[] usings)
      {
         return r => r.AddUsingDirectives(usings).Any();
      }

      private bool AddINotifyPropertyChanged(IClass cl)
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
         return true;
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


   }
}
