namespace RoslynDomExampleTests
{
   //[TestClass]
   //public class Walkthroughs_2_INotifyPropertyChanged
   //{
   //   private string inputDirectory = @"..\..\..\RoslynDom";
   //   private string outputDirectory = @"..\..\Walkthrough2_Updated_Files";
   //   private string[] subDirectories = new string[] { @"BasesAndBaseHelpers", @"Implementations", @"StatementImplementations" };

   //   [TestMethod]
   //   public void Walkthroughs_1_update_using_for_data_annotation()
   //   {
   //      DoUpdate(GetFilePairs("*.cs", inputDirectory, outputDirectory, subDirectories),
   //         GetRoot, AddUsingStatement("System.ComponentModel.DataAnnotations"));
   //   }

   //   [TestMethod]
   //   public void Walkthroughs_2_update_notify_property_changed()
   //   {
   //      DoUpdate(GetFilePairs("*.cs", inputDirectory, outputDirectory, subDirectories),
   //         GetRDomClasses, AddINotifyPropertyChanged);
   //   }

   //   private bool DoUpdate<TChange>(IEnumerable<Tuple<string, string>> filePairs,
   //                           Func<IRoot, IEnumerable<TChange>> getItemsToChange,
   //                           Func<TChange, bool> changeItems)
   //      where TChange : IDom
   //   {
   //      var didAnythingChange = false;
   //      foreach (var pair in filePairs)
   //      {
   //         if (UpdateUtilities .DoUpdateOnFile(pair.Item1, pair.Item2, getItemsToChange, changeItems))
   //         { didAnythingChange = true; }
   //      }
   //      return didAnythingChange;
   //   }

   //   private IEnumerable<Tuple<string, string>> GetFilePairs(string pattern,
   //            string inputDirectory, string outputDirectory, params string[] subDirectories)
   //   {
   //      IEnumerable<Tuple<string, string>> filePairs = new List<Tuple<string, string>>();
   //      foreach (var subDir in subDirectories)
   //      {
   //         filePairs = filePairs.Union(
   //            Directory
   //               .GetFiles(Path.Combine(inputDirectory, subDir), pattern)
   //               .Select(x => Tuple.Create(x, Path.Combine(outputDirectory, subDir, Path.GetFileName(x)))));
   //      }

   //      return filePairs;
   //   }

   //   private static IEnumerable<IRoot> GetRoot(IRoot root)
   //   {
   //      return new List<IRoot>() { root };
   //   }

   //   private static IEnumerable<IClass> GetRDomClasses(IRoot root)
   //   {
   //      return root.RootClasses
   //                     .Where(x => x.BaseType != null
   //         && ((x.BaseType.Name == "RDomBaseType" && x.BaseType.TypeArguments.Count() == 1)
   //               || (x.BaseType.Name.StartsWith("RDomBase") && x.BaseType.TypeArguments.Count() == 2)));
   //   }

   //   #region Using Statement changes
   //   private Func<IRoot, bool> AddUsingStatement(params string[] usings)
   //   {
   //      return r => r.AddUsingDirectives(usings).Any();
   //   }
   //   #endregion

   //   #region NotifyPropertyChanged
   //   private bool AddINotifyPropertyChanged(IClass cl)
   //   {
   //      var notifyingProps = cl.Properties
   //                           .Where(x => x.CanSet && x.CanGet
   //                                 && !x.SetAccessor.Statements.Any()
   //                                 && !x.GetAccessor.Statements.Any()
   //                                 && x.AccessModifier == AccessModifier.Public);
   //      foreach (var prop in notifyingProps)
   //      {
   //         UpdateProperty(prop);
   //      }
   //      return true;
   //   }

   //   private void UpdateProperty(IProperty prop)
   //   {
   //      // Add the field without further checks because the programmer will find and resolve
   //      // things like naming collisions
   //      var parent = prop.Parent as ITypeMemberContainer;
   //      var fieldName = StringUtilities.CamelCase(prop.Name);
   //      var field = new RDomField(fieldName, prop.ReturnType, declaredAccessModifier: AccessModifier.Private);
   //      FixWhitespace(field, prop);
   //      field.Whitespace2Set.Add(new Whitespace2(prop.Whitespace2Set.First().Copy()));
   //      parent.MembersAll.InsertOrMoveBefore(prop, field);
   //      UpdatePropertyGet(prop, fieldName);
   //      UpdatePropertySet(prop, fieldName);
   //   }

   //   private void FixWhitespace(RDomField field, IProperty prop)
   //   {
   //      // TODO: This is rather detailed because of featuresnot yet in the whitespace system
   //      var leading = prop.Whitespace2Set[LanguageElement.Public].LeadingWhitespace;
   //      field.Whitespace2Set[LanguageElement.Private] = new Whitespace2(LanguageElement.Private, leading, " ", "");
   //   }

   //   private void UpdatePropertyGet(IProperty prop, string fieldName)
   //   {
   //      var retExpression = RDomCSharp.Factory.ParseExpression(fieldName);
   //      var statement = new RDomReturnStatement(retExpression, true);
   //      prop.GetAccessor.StatementsAll.AddOrMove(statement);
   //      prop.GetAccessor.EnsureNewLineAfter();
   //   }

   //   private void UpdatePropertySet(IProperty prop, string fieldName)
   //   {
   //      var expression = RDomCSharp.Factory.ParseExpression(string.Format("SetProperty(ref {0}, value)", fieldName));
   //      var statement = new RDomInvocationStatement(expression, true);
   //      prop.SetAccessor.StatementsAll.AddOrMove(statement);
   //      prop.GetAccessor.EnsureNewLineAfter();
   //   }
   //   #endregion

   //   //#region AddConstructorIfNeeded

   //   //private bool AddConstructorIfNeeded(IClass cl)
   //   //{
   //   //   var constructors = cl.Constructors
   //   //                     .Where(x => (x.Parameters.First().Type.Name != "SyntaxNode"
   //   //                               && x.Parameters.First().Type.Name != cl.Name));
   //   //   if (constructors.Any()) { return false; }

   //   //   RDomConstructor constructor = CreateRDomConstructor(cl);
   //   //   var properties = cl.Properties.Where(x => x.CanSet && x.CanGet);
   //   //   var assignments = new List<IAssignmentStatement>();
   //   //   var altConstructorPairs = new List<Tuple<RDomParameter, RDomParameter, RDomArgument>>();

   //   //   foreach (var prop in properties)
   //   //   {
   //   //      RDomParameter param = CreateParameter(assignments, altConstructorPairs, prop);
   //   //      constructor.Parameters.AddOrMove(param);
   //   //      constructor.StatementsAll.AddOrMoveRange(assignments);
   //   //   }
   //   //   if (altConstructorPairs.Any())
   //   //   {
   //   //      IConstructor altConstructor = CreateAlternateConstructor(constructor, altConstructorPairs);
   //   //      cl.InsertAfterInitialFields(altConstructor);
   //   //   }
   //   //   return true;
   //   //}

   //   //private static IConstructor CreateAlternateConstructor(RDomConstructor constructor, List<Tuple<RDomParameter, RDomParameter, RDomArgument>> altConstructorPairs)
   //   //{
   //   //   var altConstructor = constructor.Copy();
   //   //   var argList = new List<RDomArgument>();
   //   //   var replaceList = new List<Tuple<IParameter, RDomParameter>>();
   //   //   foreach (var param in altConstructor.Parameters)
   //   //   {
   //   //      var switchTuples = altConstructorPairs.Where(x => x.Item1.Name == param.Name);
   //   //      if (switchTuples.Any())
   //   //      {
   //   //         var switchTuple = switchTuples.First();
   //   //         replaceList.Add(Tuple.Create(param, switchTuple.Item2));
   //   //         argList.Add(switchTuple.Item3);
   //   //      }
   //   //      else
   //   //      { argList.Add(new RDomArgument(RDomCSharp.Factory.ParseExpression(param.Name))); }
   //   //   }
   //   //   foreach (var tuple in replaceList)
   //   //   { altConstructor.Parameters.Replace(tuple.Item1, tuple.Item2); }
   //   //   altConstructor.ConstructorInitializerType = ConstructorInitializerType.This;
   //   //   altConstructor.InitializationArguments.Clear();
   //   //   altConstructor.InitializationArguments.AddOrMoveRange(argList);
   //   //   return altConstructor;
   //   //}

   //   //private RDomParameter CreateParameter(List<IAssignmentStatement> assignments, List<Tuple<RDomParameter, RDomParameter, RDomArgument>> altConstructorPairs, IProperty prop)
   //   //{
   //   //   var paramName =( prop.Name.StartsWith("_")  ? "" : "_") + StringUtilities.CamelCase(prop.Name);
   //   //   var type = prop.PropertyType.Copy();
   //   //   var param = new RDomParameter(paramName, type);
   //   //   assignments.Add(new RDomAssignmentStatement(
   //   //            RDomCSharp.Factory.ParseExpression(prop.Name),
   //   //            RDomCSharp.Factory.ParseExpression(paramName)));
   //   //   if (!(prop.Attributes.Any(x => x.Name == "Required")))
   //   //   {
   //   //      param.DefaultValue = GetDefaultValue(prop.PropertyType);
   //   //   }
   //   //   if (prop.PropertyType.Name == "IReferencedType")
   //   //   {
   //   //      var paramNewName = paramName + "Name";
   //   //      var paramNew = new RDomParameter(paramNewName, new RDomReferencedType("System.String", displayAlias: true));
   //   //      var argNew = new RDomArgument(RDomCSharp.Factory.ParseExpression(string.Format("new RDomReferencedType({0})", paramNewName)));
   //   //      altConstructorPairs.Add(Tuple.Create(param, paramNew, argNew));
   //   //   }

   //   //   return param;
   //   //}

   //   //private static RDomConstructor CreateRDomConstructor(IClass cl)
   //   //{
   //   //   var constructor = new RDomConstructor(cl.Name, AccessModifier.Public, constructorInitializerType: ConstructorInitializerType.This);
   //   //   var nullParam = new RDomArgument(RDomCSharp.Factory.ParseExpression("null"));
   //   //   constructor.InitializationArguments.AddOrMoveRange(new IArgument[] { nullParam, nullParam.Copy(), nullParam.Copy() });
   //   //   cl.InsertAfterInitialFields(constructor);
   //   //   return constructor;
   //   //}

   //   //private object GetDefaultValue(IReferencedType propertyType)
   //   //{
   //   //   if (propertyType.Name == "Boolean") return false;
   //   //   if (propertyType.Name == "String") return null;
   //   //   if (propertyType.Name == "AccessModifier") return AccessModifier.Private;
   //   //   if (propertyType.Name == "IStructuredDocumentation") return null;
   //   //   if (propertyType.Name == "Boolean") return false;
   //   //   if (propertyType.Name == "Int32") return false;
   //   //   if (propertyType.Name == "Variance") return Variance.None;
   //   //   if (propertyType.Name == "LiteralKind") return LiteralKind.Unknown;
   //   //   if (propertyType.Name == "Type") return null;
   //   //   if (propertyType.Name == "AssignmentOperator") return AssignmentOperator.Equals;
   //   //   if (propertyType.Name == "ConstructorInitializerType") return ConstructorInitializerType.None;
   //   //   if (propertyType.Name == "IExpression") return null;

   //   //   // TODO: FIgure out how to specify default as that is all you can do with Guid
   //   //   if (propertyType.Name == "Boolean") return false;

   //   //   if (propertyType.Name == "IReferencedType") return false;      // force an error
   //   //   if (propertyType.Name == "IVariableDeclaration") return false; // force an error
   //   //   if (propertyType.Name == "VariableKind") return false;         // force an error
   //   //   if (propertyType.Name == "ExpressionType") return false;       // force an error
   //   //   if (propertyType.Name == "Operator") return false;             // force an error
   //   //   if (propertyType.Name == "INamedTypeSymbol") return false;     // force an error
   //   //   return false;                                                  // force an error
   //   //}

   //   //#endregion

   //}
}