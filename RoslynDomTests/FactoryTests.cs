using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
   [TestClass]
   public class FactoryTests
   {
      // Test categories
      private const string GeneralFactoryCategory = "GeneralFactory";
      private const string SymbolCategory = "Symbol";
      private const string TypedSymbolCategory = "TypedSymbol";
      private const string RawSyntaxCategory = "RawItem";
      private const string TypedSyntaxCategory = "TypedSyntax";
      private const string StemContainerCategory = "StemContainerSyntax";

      #region general factory tests
      [TestMethod, TestCategory(GeneralFactoryCategory), TestCategory("_Common")]
      public void Can_get_root_from_file()
      {
         var fileName = @"..\..\TestFile.cs";
         IRoot root = RDom.CSharp.LoadFromFile(fileName);
         Assert.IsNotNull(root);
         Assert.AreEqual(1, root.ChildNamespaces.Count());
         Assert.AreEqual(fileName, root.FilePath);
      }

      [TestMethod, TestCategory(GeneralFactoryCategory)]
      public void Can_get_root_from_string()
      {
         var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { }
                        ";
         IRoot root = RDom.CSharp.Load(csharpCode);
         Assert.IsNotNull(root);
         Assert.AreEqual(1, root.ChildNamespaces.Count());
         Assert.IsNotNull(root.RawItem);
         Assert.IsNotNull(root.OriginalRawItem);
         Assert.IsNotNull(((RDomRoot)root).TypedSyntax);
         Assert.IsNotNull(((RDomRoot)root).OriginalTypedSyntax);
      }

      [TestMethod, TestCategory(GeneralFactoryCategory)]
      public void Can_get_root_from_syntaxtree()
      {
         var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { }
                        ";
         var tree = CSharpSyntaxTree.ParseText(csharpCode);
         var root = RDom.CSharp.Load(tree);
         Assert.IsNotNull(root);
         Assert.AreEqual(1, root.ChildNamespaces.Count());
      }

      [TestMethod, TestCategory(GeneralFactoryCategory)]
      public void Can_get_root_from_document()
      {
         var slnFile = FileSupport.GetNearestSolution(Environment.CurrentDirectory);

         var ws = MSBuildWorkspace.Create();
         // For now: wait for the result
         var solution = ws.OpenSolutionAsync(slnFile).Result;
         var project = solution.Projects.Where(x => x.Name == "RoslynDomExampleTests").FirstOrDefault();
         var document = project.Documents.Where(x => x.Name == "WorkspaceTests.cs").FirstOrDefault();
         Assert.IsNotNull(document);
         var root = RDom.CSharp.Load(document);
         Assert.IsNotNull(root);
         Assert.AreEqual(1, root.ChildNamespaces.Count());
      }

      [TestMethod, TestCategory(GeneralFactoryCategory)]
      public void Can_get_root_group_from_project()
      {
         var slnFile = FileSupport.GetNearestSolution(Environment.CurrentDirectory);

         var ws = MSBuildWorkspace.Create();
         // For now: wait for the result
         var solution = ws.OpenSolutionAsync(slnFile).Result;
         var project = solution.Projects.Where(x => x.Name == "RoslynDomExampleTests").FirstOrDefault();
         var rootGroup = RDom.CSharp.LoadGroup(project);
         Assert.IsNotNull(rootGroup);
         Assert.AreEqual(1, rootGroup.Roots.First().ChildNamespaces.Count());
      }


      [TestMethod, TestCategory(GeneralFactoryCategory)]
      public void Can_get_root_from_string_with_invalid_code()
      {
         var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { 
                            publi classX x {}
                            }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.IsNotNull(root);
         Assert.AreEqual(1, root.ChildNamespaces.Count());
      }

      [TestMethod, TestCategory(GeneralFactoryCategory)]
      public void Can_get_root_from_string_with_invalid_code2()
      {
         var csharpCode = @"foo";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.IsNotNull(root);
         Assert.AreEqual(0, root.Descendants.Count());
      }

      [TestMethod, TestCategory(GeneralFactoryCategory)]
      public void BuildSyntax_returns_null_for_null()
      {
         Assert.IsNull(RDom.CSharp.GetSyntaxNode(null));
      }

      // TODO: Should this test pass, or be removed?
      //[TestMethod, TestCategory(GeneralFactoryCategory)]
      //public void BuildSyntaxGroup_returns_empty_list_for_null()
      //{
      //    Assert.IsFalse(RDomCSharp.Factory.BuildSyntaxGroup(null).Any());
      //}


      [TestMethod, TestCategory(GeneralFactoryCategory)]
      public void Forced_corporation_returns_empty_list_on_null()
      {
         // Do not access the corporation directly as that's the purpose of the 
         // language specific factory. However, I want to test these two side
         // cases. 
         var corp = new RDomCorporation(LanguageNames.CSharp, RDom.CSharp);
         var result1 = corp.GetSyntaxNodes(null);
         Assert.IsFalse(result1.Any());
      }

      [TestMethod, TestCategory(GeneralFactoryCategory)]
      [ExpectedException(typeof(InvalidOperationException))]
      public void Forced_corporation_throws_on_non_implemented_item()
      {
         Assert.Inconclusive();
         var csharpCode = @"public classX x {}";
         var root = RDom.CSharp.Load(csharpCode);
         // Do not access the corporation directly as that's the purpose of the 
         // language specific factory. However, I want to test these two side
         // cases. 
         var corp = new RDomCorporation(LanguageNames.CSharp, RDom.CSharp);
         var a = new ClassA();
         var result1 = corp.GetSyntaxNodes(a);
      }

      [TestMethod, TestCategory(GeneralFactoryCategory)]
      public void Can_get_root_group_from_strings()
      {
         var csharpCode1 = @"
                  using System.Diagnostics.Tracing;
                  namespace testing.Namespace1
                  { 
                  public class A{}
                  }";

         var csharpCode2 = @"
                  using System.Diagnostics.Tracing;
                  namespace testing.Namespace1
                  { 
                  public class B : A
                     {}
                  }";
         var rootGroup = RDom.CSharp.LoadGroup(csharpCode1, csharpCode2);
         Assert.IsNotNull(rootGroup);
         var root = rootGroup.Roots.First();
         Assert.AreEqual(1, root.ChildNamespaces.Count());
         Assert.AreEqual("testing", root.ChildNamespaces.First().Name);
         Assert.AreEqual(1, root.RootClasses.Count());
         Assert.AreEqual("A", root.RootClasses.First().Name);
         root = rootGroup.Roots.ElementAt(1);
         Assert.AreEqual(1, root.ChildNamespaces.Count());
         Assert.AreEqual("testing", root.ChildNamespaces.First().Name);
         Assert.AreEqual(1, root.RootClasses.Count());
         var cl = root.RootClasses.First();
         Assert.AreEqual("B", cl.Name);
         Assert.AreEqual("A", cl.BaseType.Name);
         Assert.IsNotNull(cl.BaseType.Type);
         Assert.AreEqual("A", cl.BaseType.Type.Name);
      }

      [TestMethod, TestCategory(GeneralFactoryCategory)]
      public void Can_get_root_group_from_project_via_solution()
      {
         var slnFile = FileSupport.GetNearestSolution(Environment.CurrentDirectory);

         var ws = MSBuildWorkspace.Create();
         // For now: wait for the result
         var solution = ws.OpenSolutionAsync(slnFile).Result;
         var project = solution.Projects.Where(x => x.Name == "RoslynDom").FirstOrDefault();
         // I don't want to test much about the project - because things will change
         //Assert.IsNotNull(project);
         //var trees = new List<SyntaxTree>();
         //foreach (var doc in project.Documents )
         //{
         //   // TODO: consider how async should fit in - not in the test, it belongs deeper in the system
         //   var tree = doc.GetSyntaxTreeAsync().Result;
         //   trees.Add(tree);
         //}

         var compilation = project.GetCompilationAsync().Result;
         var globalNamespace = compilation.GlobalNamespace;
         var reference = compilation.GetTypeByMetadataName("RoslynDom.RDomBase");
         var method = reference.GetMembers().ElementAt(11);
         var span = method.DeclaringSyntaxReferences.First().Span;
         var tree = method.DeclaringSyntaxReferences.First().SyntaxTree;
         var syntax = method.DeclaringSyntaxReferences.First().GetSyntax();


         // var rootGroup = RDom.CSharp.LoadGroup(compilation, trees.ToArray());
         var rootGroup = RDom.CSharp.Load(compilation);

         Assert.IsNotNull(rootGroup);
         var classes = rootGroup.Roots
                           .SelectMany(x => x.RootClasses)
                           .ToList();
         Assert.IsTrue(classes.Count() > 50);
         var type = classes.First().BaseType.Type;
         Assert.IsInstanceOfType(type, typeof(RoslynDom.RDomBase));
      }


      // Class just for test
      private class ClassA : IDom
      {
         public IEnumerable<IDom> Ancestors
         { get { throw new NotImplementedException(); } }

         public IEnumerable<IDom> AncestorsAndSelf
         { get { throw new NotImplementedException(); } }

         public IEnumerable<IDom> Children
         { get { throw new NotImplementedException(); } }

         public IEnumerable<IDom> Descendants
         { get { throw new NotImplementedException(); } }

         public IEnumerable<IDom> DescendantsAndSelf
         { get { throw new NotImplementedException(); } }

         public bool NeedsFormatting
         {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
         }

         public object OriginalRawItem
         { get { throw new NotImplementedException(); } }

         public IDom Parent
         { get { throw new NotImplementedException(); } }

         public object RawItem
         { get { throw new NotImplementedException(); } }

         public Whitespace2Collection Whitespace2Set
         { get { throw new NotImplementedException(); } }

         public void EnsureLeading(string whitespace)
         {
            throw new NotImplementedException();
         }

         public void EnsureNewLineAfter()
         { throw new NotImplementedException(); }

         public bool Matches(IDom other)
         { throw new NotImplementedException(); }

         public string ReportHierarchy()
         { throw new NotImplementedException(); }

         public object RequestValue(string propertyName)
         { throw new NotImplementedException(); }

         public object RequestValue(string propertyName, bool searchUpLogicalTree)
         { throw new NotImplementedException(); }

         public bool SameIntent<T>(T other) where T : class
         { throw new NotImplementedException(); }

      }
      #endregion

      #region symbol tests
      [TestMethod, TestCategory(SymbolCategory)]
      public void Can_get_symbol_for_namespace()
      {
         var csharpCode = @"
                        namespace testing.Namespace1
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((IRoslynHasSymbol)root.ChildNamespaces.First()).Symbol;
         Assert.IsNotNull(symbol);
      }

      [TestMethod, TestCategory(SymbolCategory)]
      public void Can_get_symbol_for_class()
      {
         var csharpCode = @"
                        public class MyClass
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((IRoslynHasSymbol)root.Classes.First()).Symbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("MyClass", symbol.Name);
      }

      [TestMethod, TestCategory(SymbolCategory)]
      public void Can_get_symbol_for_enum()
      {
         var csharpCode = @"
                        public enum MyEnum
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((IRoslynHasSymbol)root.Enums.First()).Symbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("MyEnum", symbol.Name);
      }

      [TestMethod, TestCategory(SymbolCategory)]
      public void Can_get_symbol_for_struct()
      {
         var csharpCode = @"
                        public struct MyStruct
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((IRoslynHasSymbol)root.Structures.First()).Symbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("MyStruct", symbol.Name);
      }

      [TestMethod, TestCategory(SymbolCategory)]
      public void Can_get_symbol_for_interface()
      {
         var csharpCode = @"
                        public interface MyInterface
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((IRoslynHasSymbol)root.Interfaces.First()).Symbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("MyInterface", symbol.Name);
      }


      [TestMethod, TestCategory(SymbolCategory)]
      public void Can_get_symbol_for_field()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myField; }";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((IRoslynHasSymbol)root.Classes.First().Fields.First()).Symbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("myField", symbol.Name);
      }

      [TestMethod, TestCategory(SymbolCategory)]
      public void Can_get_symbol_for_property()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myProperty { get; } }";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((IRoslynHasSymbol)root.Classes.First().Properties.First()).Symbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("myProperty", symbol.Name);
      }

      [TestMethod, TestCategory(SymbolCategory)]
      public void Can_get_symbol_for_method()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((IRoslynHasSymbol)root.Classes.First().Methods.First()).Symbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("myMethod", symbol.Name);
      }

      [TestMethod, TestCategory(SymbolCategory)]
      public void Can_get_symbol_for_parameter()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((IRoslynHasSymbol)root.Classes.First().Methods.First().Parameters.First()).Symbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("x", symbol.Name);
      }

      [TestMethod, TestCategory(SymbolCategory)]
      public void Can_get_symbol_for_nestedType()
      {
         var csharpCode = @"
                        public class MyClass
                        { public class MyNestedClass {  } }";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((IRoslynHasSymbol)root.Classes.First().Types.First()).Symbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("MyNestedClass", symbol.Name);
      }
      #endregion

      #region typed symbol tests
      [TestMethod, TestCategory(TypedSymbolCategory)]
      public void Can_get_typed_symbol_for_namespace()
      {
         var csharpCode = @"
                        namespace testing.Namespace1
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((RDomNamespace)root.ChildNamespaces.First()).TypedSymbol as INamespaceSymbol;
         Assert.AreEqual("Namespace1", symbol.Name);
         Assert.IsNotNull(symbol);
      }

      [TestMethod, TestCategory(TypedSymbolCategory)]
      public void Can_get_typed_symbol_for_class()
      {
         var csharpCode = @"
                        public class MyClass
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((RDomClass)root.Classes.First()).TypedSymbol as INamedTypeSymbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("MyClass", symbol.Name);
      }

      [TestMethod, TestCategory(TypedSymbolCategory)]
      public void Can_get_typed_symbol_for_enum()
      {
         var csharpCode = @"
                        public enum MyEnum
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((RDomEnum)root.Enums.First()).TypedSymbol as INamedTypeSymbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("MyEnum", symbol.Name);
      }

      [TestMethod, TestCategory(TypedSymbolCategory)]
      public void Can_get_typed_symbol_for_struct()
      {
         var csharpCode = @"
                        public struct MyStruct
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((RDomStructure)root.Structures.First()).TypedSymbol as INamedTypeSymbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("MyStruct", symbol.Name);
      }

      [TestMethod, TestCategory(TypedSymbolCategory)]
      public void Can_get_typed_symbol_for_interface()
      {
         var csharpCode = @"
                        public interface MyInterface
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((RDomInterface)root.Interfaces.First()).TypedSymbol as INamedTypeSymbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("MyInterface", symbol.Name);
      }


      [TestMethod, TestCategory(TypedSymbolCategory)]
      public void Can_get_typed_symbol_for_field()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myField; }";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((RDomField)root.Classes.First().Fields.First()).TypedSymbol as IFieldSymbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("myField", symbol.Name);
      }

      [TestMethod, TestCategory(TypedSymbolCategory)]
      public void Can_get_typed_symbol_for_property()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myProperty { get; } }";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((RDomProperty)root.Classes.First().Properties.First()).TypedSymbol as IPropertySymbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("myProperty", symbol.Name);
      }

      [TestMethod, TestCategory(TypedSymbolCategory)]
      public void Can_get_typed_symbol_for_method()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((RDomMethod)root.Classes.First().Methods.First()).TypedSymbol as IMethodSymbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("myMethod", symbol.Name);
      }

      [TestMethod, TestCategory(TypedSymbolCategory)]
      public void Can_get_typed_symbol_for_nestedType()
      {
         var csharpCode = @"
                        public class MyClass
                        { public class MyNestedClass {  } }";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((RDomClass)root.Classes.First().Types.First()).TypedSymbol as INamedTypeSymbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("MyNestedClass", symbol.Name);
      }
      #endregion

      #region RawItem tests
      [TestMethod, TestCategory(RawSyntaxCategory)]
      public void Can_get_rawSyntax_for_namespace()
      {
         var csharpCode = @"
                        namespace testing.Namespace1
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var RawItem = root.ChildNamespaces.First().RawItem;
         Assert.IsNotNull(RawItem);
      }

      [TestMethod, TestCategory(RawSyntaxCategory)]
      public void Can_get_rawSyntax_for_class()
      {
         var csharpCode = @"
                        public class MyClass
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var RawItem = root.Classes.First().RawItem;
         Assert.IsNotNull(RawItem);
         Assert.IsTrue(RawItem is ClassDeclarationSyntax);
      }

      [TestMethod, TestCategory(RawSyntaxCategory)]
      public void Can_get_rawSyntax_for_enum()
      {
         var csharpCode = @"
                        public enum MyEnum
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var RawItem = root.Enums.First().RawItem;
         Assert.IsNotNull(RawItem);
         Assert.IsTrue(RawItem is EnumDeclarationSyntax);
      }

      [TestMethod, TestCategory(RawSyntaxCategory)]
      public void Can_get_rawSyntax_for_struct()
      {
         var csharpCode = @"
                        public struct MyStruct
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var RawItem = root.Structures.First().RawItem;
         Assert.IsNotNull(RawItem);
         Assert.IsTrue(RawItem is StructDeclarationSyntax);
      }

      [TestMethod, TestCategory(RawSyntaxCategory)]
      public void Can_get_rawSyntax_for_interface()
      {
         var csharpCode = @"
                        public interface MyInterface
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var RawItem = root.Interfaces.First().RawItem;
         Assert.IsNotNull(RawItem);
         Assert.IsTrue(RawItem is InterfaceDeclarationSyntax);
      }

      [TestMethod, TestCategory(RawSyntaxCategory)]
      public void Can_get_rawSyntax_for_field()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myField; }";
         var root = RDom.CSharp.Load(csharpCode);
         var RawItem = root.Classes.First().Fields.First().RawItem;
         Assert.IsNotNull(RawItem);
         Assert.IsTrue(RawItem is VariableDeclaratorSyntax);
      }

      [TestMethod, TestCategory(RawSyntaxCategory)]
      public void Can_get_rawSyntax_for_property()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myProperty { get; } }";
         var root = RDom.CSharp.Load(csharpCode);
         var RawItem = root.Classes.First().Properties.First().RawItem;
         Assert.IsNotNull(RawItem);
         Assert.IsTrue(RawItem is PropertyDeclarationSyntax);
      }

      [TestMethod, TestCategory(RawSyntaxCategory)]
      public void Can_get_rawSyntax_for_method()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
         var root = RDom.CSharp.Load(csharpCode);
         var RawItem = root.Classes.First().Methods.First().RawItem;
         Assert.IsNotNull(RawItem);
         Assert.IsTrue(RawItem is MethodDeclarationSyntax);
      }

      [TestMethod, TestCategory(RawSyntaxCategory)]
      public void Can_get_rawSyntax_for_nestedType()
      {
         var csharpCode = @"
                        public class MyClass
                        { public class MyNestedClass {  } }";
         var root = RDom.CSharp.Load(csharpCode);
         var RawItem = root.Classes.First().Types.First().RawItem;
         Assert.IsNotNull(RawItem);
         Assert.IsTrue(RawItem is ClassDeclarationSyntax);
      }

      [TestMethod, TestCategory(RawSyntaxCategory)]
      public void Can_get_rawSyntax_for_referencedType()
      {
         var csharpCode = @"
                        public class Foo
{
public string Bar1;
public string Bar2 {get;};
public string Bar3() {};
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var field = root.Classes.First().Fields.First();
         var retType = field.ReturnType;
         var rawItem = retType.RawItem;
         Assert.IsNotNull(rawItem, "field");
         Assert.IsTrue(rawItem is PredefinedTypeSyntax, "field");
         var property = root.Classes.First().Properties.First();
         retType = property.ReturnType;
         rawItem = retType.RawItem;
         Assert.IsNotNull(rawItem, "property");
         Assert.IsTrue(rawItem is PredefinedTypeSyntax, "property");
         var method = root.Classes.First().Methods.First();
         retType = method.ReturnType;
         rawItem = retType.RawItem;
         Assert.IsNotNull(rawItem, "method");
         Assert.IsTrue(rawItem is PredefinedTypeSyntax, "method");
      }
      #endregion

      #region typedSyntax tests
      [TestMethod, TestCategory(TypedSyntaxCategory)]
      public void Can_get_typedSyntax_for_namespace()
      {
         var csharpCode = @"
                        namespace testing.Namespace1
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var typedSyntax = ((RDomNamespace)root.ChildNamespaces.First()).TypedSyntax;
         Assert.IsNotNull(typedSyntax);
         Assert.IsTrue(typedSyntax is NamespaceDeclarationSyntax);
      }

      [TestMethod, TestCategory(TypedSyntaxCategory)]
      public void Can_get_typedSyntax_for_class()
      {
         var csharpCode = @"
                        public class MyClass
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var typedSyntax = ((RDomClass)root.Classes.First()).TypedSyntax;
         Assert.IsNotNull(typedSyntax);
         Assert.IsTrue(typedSyntax is ClassDeclarationSyntax);
      }

      [TestMethod, TestCategory(TypedSyntaxCategory)]
      public void Can_get_typedSyntax_for_enum()
      {
         var csharpCode = @"
                        public enum MyEnum
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var typedSyntax = ((RDomEnum)root.Enums.First()).TypedSyntax;
         Assert.IsNotNull(typedSyntax);
         Assert.IsTrue(typedSyntax is EnumDeclarationSyntax);
      }

      [TestMethod, TestCategory(TypedSyntaxCategory)]
      public void Can_get_typedSyntax_for_struct()
      {
         var csharpCode = @"
                        public struct MyStruct
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var typedSyntax = ((RDomStructure)root.Structures.First()).TypedSyntax;
         Assert.IsNotNull(typedSyntax);
         Assert.IsTrue(typedSyntax is StructDeclarationSyntax);
      }

      [TestMethod, TestCategory(TypedSyntaxCategory)]
      public void Can_get_typedSyntax_for_interface()
      {
         var csharpCode = @"
                        public interface MyInterface
                            { }
                        ";
         var root = RDom.CSharp.Load(csharpCode);
         var typedSyntax = ((RDomInterface)root.Interfaces.First()).TypedSyntax;
         Assert.IsNotNull(typedSyntax);
         Assert.IsTrue(typedSyntax is InterfaceDeclarationSyntax);
      }


      [TestMethod, TestCategory(TypedSyntaxCategory)]
      public void Can_get_typedSyntax_for_field()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myField; }";
         var root = RDom.CSharp.Load(csharpCode);
         var typedSyntax = ((RDomField)root.Classes.First().Fields.First()).TypedSyntax;
         Assert.IsNotNull(typedSyntax);
         Assert.IsTrue(typedSyntax is VariableDeclaratorSyntax);
      }

      [TestMethod, TestCategory(TypedSyntaxCategory)]
      public void Can_get_typedSyntax_for_property()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myProperty { get; } }";
         var root = RDom.CSharp.Load(csharpCode);
         var typedSyntax = ((RDomProperty)root.Classes.First().Properties.First()).TypedSyntax;
         Assert.IsNotNull(typedSyntax);
         Assert.IsTrue(typedSyntax is PropertyDeclarationSyntax);
      }

      [TestMethod, TestCategory(TypedSyntaxCategory)]
      public void Can_get_typedSyntax_for_method()
      {
         var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
         var root = RDom.CSharp.Load(csharpCode);
         var typedSyntax = ((RDomMethod)root.Classes.First().Methods.First()).TypedSyntax;
         Assert.IsNotNull(typedSyntax);
         Assert.IsTrue(typedSyntax is MethodDeclarationSyntax);
      }

      [TestMethod, TestCategory(TypedSyntaxCategory)]
      public void Can_get_typedSyntax_for_nestedType()
      {
         var csharpCode = @"
                        public class MyClass
                        { public class MyNestedClass {  } }";
         var root = RDom.CSharp.Load(csharpCode);
         var typedSyntax = ((RDomClass)root.Classes.First()).TypedSyntax;
         Assert.IsNotNull(typedSyntax);
         Assert.IsTrue(typedSyntax is ClassDeclarationSyntax);
      }
      #endregion

      #region stemContainer
      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_namespaces()
      {
         var csharpCode = @"
                        namespace testing.Namespace1
                            { }

                        namespace testing.Namespace2
                            { }
";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.AreEqual(2, root.ChildNamespaces.Count());
      }

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_classes()
      {
         var csharpCode = @"
                        class Class1
                            { }
 class Class2
                            { }
class Class3
                            { }
";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.AreEqual(3, root.Classes.Count());
      }

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_interfaces()
      {
         var csharpCode = @"
                        interface Interface
                            {

private string Foo{get;}}
";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.AreEqual(1, root.Interfaces.Count());
      }

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_structures()
      {
         var csharpCode = @"
                        struct Struct1
                            { 
string Foo{get;}}
                        struct Struct1
                            { }
";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.AreEqual(2, root.Structures.Count());
      }

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_enums()
      {
         var csharpCode = @"
                        enum A
                            { }
                        enum B
                            { }
";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.AreEqual(2, root.Enums.Count());
      }

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_using_directives()
      {
         var csharpCode = @"
                        using A;
                        using B;
";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.AreEqual(2, root.UsingDirectives.Count());
      }

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_usingdirectives_in_namespaces()
      {
         var csharpCode = @"
namespace Foo
{
                        using A;
                        using B;
}
";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.AreEqual(2, root.Namespaces.First().UsingDirectives.Count());
      }

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_types()
      {
         var csharpCode = @"
                        enum A
                            { }
                        enum B
                            { }
                        class C
                            { }
                        interface D
                            { }
                        interface E
                            { }
                        interface F
                            { }
                        struct G
                            { }
                      struct G
                            { }
                      struct H
                            { }
                      struct I
                            { }
                      struct J
                            { }";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.AreEqual(11, root.Types.Count());
      }

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_members_with_comments_and_whitespace_from_stemContainer()
      {
         // This test is sensitive to whitespace changes on the multiline comment 
         var csharpCode =
                   @"enum A
                            { }

                        enum B
                            { }

                        // Single line comment with whitespace
                        class C
                            { }
                        interface D
                            { }
                        // Single line comment without whitespace
                        interface E
                            { }
                        /* Multiline comment on single line */
                        interface F
                            { }
                        struct G
                            { }
                        /* Multiline comment 
                       on multiple line */
                      struct H
                            { }
                      struct I
                            { }
                      struct J
                            { }
                      struct K
                            { }";
         var root = RDom.CSharp.Load(csharpCode);
         Assert.AreEqual(11, root.Types.Count());

         // Check that all were found
         var members = root.StemMembersAll.ToArray();
         Assert.AreEqual(17, members.Count());

         // Check some reality stuff
         Assert.IsInstanceOfType(members[3], typeof(IVerticalWhitespace));
         Assert.IsInstanceOfType(members[4], typeof(IComment));
         Assert.IsInstanceOfType(members[7], typeof(IComment));
         Assert.IsInstanceOfType(members[9], typeof(IComment));
         Assert.IsInstanceOfType(members[12], typeof(IComment));
         Assert.AreEqual(1, ((IVerticalWhitespace)members[3]).Count);
         Assert.IsFalse(((IVerticalWhitespace)members[3]).IsElastic);
         var expected = "Single line comment with whitespace";
         Assert.AreEqual(expected, ((IComment)members[4]).Text);
         Assert.IsFalse(((IComment)members[4]).IsMultiline);
         expected = "Single line comment without whitespace";
         Assert.AreEqual(expected, ((IComment)members[7]).Text);
         Assert.IsFalse(((IComment)members[7]).IsMultiline);
         expected = "Multiline comment on single line";
         Assert.AreEqual(expected, ((IComment)members[9]).Text);
         Assert.IsTrue(((IComment)members[9]).IsMultiline);
         expected = "Multiline comment \r\n                       on multiple line";
         Assert.AreEqual(expected, ((IComment)members[12]).Text);
         Assert.IsTrue(((IComment)members[12]).IsMultiline);

         var actual = RDom.CSharp.GetSyntaxNode(root);
         var actualCode = actual.ToFullString();
         Assert.AreEqual(csharpCode, actualCode);
      }
      #endregion

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_members_with_comments_and_whitespace_from_typeContainer()
      {
         var csharpCode = @"
                public class Bar
                {
                    private string firstName;

                        // Comment and whitespace
                    private string lastName;

                        // comment
                    public string Foo()
                    {}
                }";
         var root = RDom.CSharp.Load(csharpCode);

         // Check that all were found
         var members = root.Classes.First().MembersAll.ToArray();
         Assert.AreEqual(7, members.Count());

         // Check some reality stuff
         Assert.AreEqual(1, ((IVerticalWhitespace)members[1]).Count);
         Assert.IsFalse(((IVerticalWhitespace)members[1]).IsElastic);
         var expected = "Comment and whitespace";
         Assert.AreEqual(expected, ((IComment)members[2]).Text);
         Assert.IsFalse(((IComment)members[2]).IsMultiline);

         // Check output
         var actual = RDom.CSharp.GetSyntaxNode(root);
         Assert.AreEqual(csharpCode, actual.ToFullString());
      }

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_members_with_region()
      {
         Assert.Inconclusive();
         var csharpCode = @"
             #region Fred   
                public class Bar
                {
                    private string firstName;

                        // Comment and whitespace
                    private string lastName;
   
                        // comment
                    public string Foo()
                    {}
                }
            #endregion";
         var root = RDom.CSharp.Load(csharpCode);

         // Check that all were found
         var members = root.Classes.First().MembersAll.ToArray();
         Assert.AreEqual(7, members.Count());

         // Check some reality stuff
         Assert.AreEqual(1, ((IVerticalWhitespace)members[1]).Count);
         Assert.IsFalse(((IVerticalWhitespace)members[1]).IsElastic);
         var expected = "Comment and whitespace";
         Assert.AreEqual(expected, ((IComment)members[2]).Text);
         Assert.IsFalse(((IComment)members[2]).IsMultiline);

         // Check output
         var actual = RDom.CSharp.GetSyntaxNode(root);
         Assert.AreEqual(csharpCode, actual.ToFullString());
      }

      [TestMethod, TestCategory(StemContainerCategory)]
      public void Can_get_members_with_comments_and_whitespace_from_statementContainer()
      {
         var csharpCode = @"
                public class Bar
                {
                    private string firstName;
                    private string lastName;

                    public string Foo()
                    {

                        var ret = lastName;
                        ret = Foo();
                        ret = ""xyz"";

                        // Comment and whitespace
                        var xx = new String('a', 4);
                        ret = ""abc"" + Foo();
                        // comment
                        if (!string.IsNullOrEmpty(firstName))
                        { ret = firstName + lastName; }
                        var x = "", "";
                        uint y = 42;
                        x = lastName + x + firstName;
                        Foo2(x);
                        return x;
                    }
                }";
         var root = RDom.CSharp.Load(csharpCode);

         // Check that all were found
         var statements = root.Classes.First().Methods.First().StatementsAll.ToArray();
         Assert.AreEqual(15, statements.Count());

         // Check some reality stuff
         Assert.AreEqual(1, ((IVerticalWhitespace)statements[4]).Count);
         Assert.IsFalse(((IVerticalWhitespace)statements[4]).IsElastic);
         var expected = "Comment and whitespace";
         Assert.AreEqual(expected, ((IComment)statements[5]).Text);
         Assert.IsFalse(((IComment)statements[5]).IsMultiline);

         var actual = RDom.CSharp.GetSyntaxNode(root);
         Assert.AreEqual(csharpCode, actual.ToFullString());
      }
   }
}
