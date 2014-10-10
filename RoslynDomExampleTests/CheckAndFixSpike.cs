using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDomExampleTests
{
   [TestClass]
   public class CheckAndFixSpike
   {
      private string inputDirectory = @"..\..\..\RoslynDom";
      private string outputDirectory = @"..\..\Walkthrough2_Updated_Files";
      private string[] subDirectories = new string[] { @"BasesAndBaseHelpers", @"Implementations", @"StatementImplementations" };

      private TriviaManager triviaManager = new TriviaManager();

      [TestMethod]
      public void Update_notify_property_changed()
      {
         var filePairs = UpdateUtilities.GetFilePairs("*.cs", inputDirectory, outputDirectory + "_2", subDirectories);
         UpdateUtilities.DoUpdate(filePairs,
               root => root.Descendants
                     .OfType<IProperty>()
                     .Where(x => UpdateNotifyPropertyChanged.ShouldNotifyPropertyChanged(x))
                     .Where(x => IsInRDomClass(x)),
               UpdateNotifyPropertyChanged.NeedsNotifyPropertyChanged,
               UpdateNotifyPropertyChanged.FixNotifyPropertyChanged);
      }

      [TestMethod]
      public void Add_RDom_constructors()
      {
         var filePairs = UpdateUtilities.GetFilePairs("*.cs", inputDirectory, outputDirectory + "_3", subDirectories);
         UpdateUtilities.DoUpdate(filePairs,
               root => GetRDomClasses(root),
               NeedsConstructor,
               AddConstructor);
      }

      private static bool IsInRDomClass(IDom prop)
      {
         if (prop.Parent == null || !(prop.Parent is IClass)) return false;
         var parent = (IClass)prop.Parent;
         if (!parent.Name.StartsWith("RDom")) return false;
         if (parent.BaseType == null || !(parent.BaseType.Name.StartsWith("RDom"))) return false;
         return true;
      }

      private static IEnumerable<IClass> GetRDomClasses(IRoot root)
      {
         return root.RootClasses
                        .Where(x => x.BaseType != null
            && ((x.BaseType.Name == "RDomBaseType" && x.BaseType.TypeArguments.Count() == 1)
                  || (x.BaseType.Name.StartsWith("RDomBase") && x.BaseType.TypeArguments.Count() == 2)));
      }

      private bool NeedsConstructor(IClass cl)
      {
         var constructors = cl.Constructors
                           .Where(x => (x.Parameters.First().Type.Name != "SyntaxNode"
                                     && x.Parameters.First().Type.Name != cl.Name));
         if (constructors.Any()) { return false; }
         return true;
      }

      private bool AddConstructor(IClass cl)
      {
         RDomConstructor constructor = CreateRDomConstructor(cl);
         triviaManager.StoreStringWhitespace(constructor, LanguageElement.ParameterEndDelimiter, "", "\r\n");
         triviaManager.StoreStringWhitespace(constructor, LanguageElement.ConstructorInitializerPrefix, "       ", "");
         var properties = cl.Properties.Where(x => x.CanSet && x.CanGet);
         var assignments = new List<IAssignmentStatement>();
         assignments.Add(new RDomAssignmentStatement(
                  RDomCSharp.Factory.ParseExpression("NeedsFormatting"),
                  RDomCSharp.Factory.ParseExpression("true")));
         triviaManager.StoreStringWhitespace(assignments.First(), LanguageElement.EndOfLine, "", "\r\n");
         var altConstructorPairs = new List<Tuple<RDomParameter, RDomParameter, RDomArgument>>();

         foreach (var prop in properties)
         {
            RDomParameter param = CreateParameter(assignments, altConstructorPairs, prop);
            constructor.Parameters.AddOrMove(param);
         }
         constructor.StatementsAll.AddOrMoveRange(assignments);
         if (altConstructorPairs.Any())
         {
            IConstructor altConstructor = CreateAlternateConstructor(constructor, altConstructorPairs);
            cl.InsertAfterInitialFields(altConstructor);
         }
         return true;
      }

      private static IConstructor CreateAlternateConstructor(RDomConstructor constructor, List<Tuple<RDomParameter, RDomParameter, RDomArgument>> altConstructorPairs)
      {
         var altConstructor = constructor.Copy();
         var argList = new List<RDomArgument>();
         var replaceList = new List<Tuple<IParameter, RDomParameter>>();
         foreach (var param in altConstructor.Parameters)
         {
            var switchTuples = altConstructorPairs.Where(x => x.Item1.Name == param.Name);
            if (switchTuples.Any())
            {
               var switchTuple = switchTuples.First();
               replaceList.Add(Tuple.Create(param, switchTuple.Item2));
               argList.Add(switchTuple.Item3);
            }
            else
            { argList.Add(new RDomArgument(RDomCSharp.Factory.ParseExpression(param.Name))); }
         }
         foreach (var tuple in replaceList)
         { altConstructor.Parameters.Replace(tuple.Item1, tuple.Item2); }
         altConstructor.ConstructorInitializerType = ConstructorInitializerType.This;
         altConstructor.InitializationArguments.Clear();
         altConstructor.InitializationArguments.AddOrMoveRange(argList);
         return altConstructor;
      }

      private RDomParameter CreateParameter(List<IAssignmentStatement> assignments, List<Tuple<RDomParameter, RDomParameter, RDomArgument>> altConstructorPairs, IProperty prop)
      {
         var paramName = (prop.Name.StartsWith("_") ? "" : "_") + StringUtilities.CamelCase(prop.Name);
         var type = prop.PropertyType.Copy();
         var param = new RDomParameter(paramName, type);
         triviaManager.StoreStringWhitespace(param, LanguageElement.Identifier, "              ", "");
         triviaManager.StoreStringWhitespace(param, LanguageElement.EndOfLine, "", "\r\n");
         var newAssignment = new RDomAssignmentStatement(
                  RDomCSharp.Factory.ParseExpression(prop.Name),
                  RDomCSharp.Factory.ParseExpression(paramName));
         triviaManager.StoreStringWhitespace(newAssignment, LanguageElement.EndOfLine, "", "\r\n");
         assignments.Add(newAssignment);
         if (!(prop.Attributes.Any(x => x.Name == "Required")))
         {
            param.DefaultValue = GetDefaultValue(prop.PropertyType);
         }
         if (prop.PropertyType.Name == "IReferencedType")
         {
            var paramNewName = paramName + "Name";
            var paramNew = new RDomParameter(paramNewName, new RDomReferencedType("System.String", displayAlias: true));
            var argNew = new RDomArgument(RDomCSharp.Factory.ParseExpression(string.Format("new RDomReferencedType({0})", paramNewName)));
            altConstructorPairs.Add(Tuple.Create(param, paramNew, argNew));
         }

         return param;
      }

      private static RDomConstructor CreateRDomConstructor(IClass cl)
      {
         var constructor = new RDomConstructor(cl.Name, AccessModifier.Public, constructorInitializerType: ConstructorInitializerType.This);
         var nullParam = new RDomArgument(RDomCSharp.Factory.ParseExpression("null"));
         constructor.InitializationArguments.AddOrMoveRange(new IArgument[] { nullParam, nullParam.Copy(), nullParam.Copy() });
         cl.InsertAfterInitialFields(constructor);
         return constructor;
      }

      private object GetDefaultValue(IReferencedType propertyType)
      {
         if (propertyType.Name == "Boolean") return false;
         if (propertyType.Name == "String") return null;
         if (propertyType.Name == "AccessModifier") return AccessModifier.Private;
         if (propertyType.Name == "IStructuredDocumentation") return null;
         if (propertyType.Name == "Boolean") return false;
         if (propertyType.Name == "Int32") return false;
         if (propertyType.Name == "Variance") return Variance.None;
         if (propertyType.Name == "LiteralKind") return LiteralKind.Unknown;
         if (propertyType.Name == "Type") return null;
         if (propertyType.Name == "AssignmentOperator") return AssignmentOperator.Equals;
         if (propertyType.Name == "ConstructorInitializerType") return ConstructorInitializerType.None;
         if (propertyType.Name == "IExpression") return null;

         // TODO: FIgure out how to specify default as that is all you can do with Guid
         if (propertyType.Name == "Boolean") return false;

         if (propertyType.Name == "IReferencedType") return false;      // force an error
         if (propertyType.Name == "IVariableDeclaration") return false; // force an error
         if (propertyType.Name == "VariableKind") return false;         // force an error
         if (propertyType.Name == "ExpressionType") return false;       // force an error
         if (propertyType.Name == "Operator") return false;             // force an error
         if (propertyType.Name == "INamedTypeSymbol") return false;     // force an error
         return false;                                                  // force an error
      }

   }
}
