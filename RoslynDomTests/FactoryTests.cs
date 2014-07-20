using System;
using System.Collections.Immutable;
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
        [TestMethod]
        [TestCategory(GeneralFactoryCategory)]
        [TestCategory("_Common")]
        public void Can_get_root_from_file()
        {
            IRoot root = RDomCSharpFactory.Factory.GetRootFromFile(@"..\..\TestFile.cs");
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Namespaces.Count());
        }

        [TestMethod]
        [TestCategory(GeneralFactoryCategory)]
        public void Can_get_root_from_string()
        {
            var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { }
                        ";
            IRoot root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Namespaces.Count());
        }

        [TestMethod]
        [TestCategory(GeneralFactoryCategory)]
        public void Can_get_root_from_syntaxtree()
        {
            var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { }
                        ";
            var tree = CSharpSyntaxTree.ParseText(csharpCode);
            var root = RDomCSharpFactory.Factory.GetRootFromSyntaxTree(tree);
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Namespaces.Count());
        }

        [TestMethod]
        [TestCategory(GeneralFactoryCategory)]
        public void Can_get_root_from_document()
        {
            var slnFile = TestUtilities.GetNearestSolution(Environment.CurrentDirectory);

            var ws = MSBuildWorkspace.Create();
            // For now: wait for the result
            var solution = ws.OpenSolutionAsync(slnFile).Result;
            var project = solution.Projects.Where(x => x.Name == "RoslynDomExampleTests").FirstOrDefault();
            var document = project.Documents.Where(x => x.Name == "WorkspaceTests.cs").FirstOrDefault();
            Assert.IsNotNull(document);
            var root = RDomCSharpFactory.Factory.GetRootFromDocument(document);
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Namespaces.Count());
        }


        [TestMethod]
        [TestCategory(GeneralFactoryCategory)]
        public void Can_get_root_from_string_with_invalid_code()
        {
            var csharpCode = @"
                        using System.Diagnostics.Tracing;
                        namespace testing.Namespace1
                            { 
                            publi classX x {}
                            }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.IsNotNull(root);
            Assert.AreEqual(1, root.Namespaces.Count());
        }
#endregion 

        #region symbol tests
        [TestMethod]
        [TestCategory(SymbolCategory)]
        public void Can_get_symbol_for_namespace()
        {
            var csharpCode = @"
                        namespace testing.Namespace1
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Namespaces.First()).Symbol;
            Assert.IsNotNull(symbol);
        }

        [TestMethod]
        [TestCategory(SymbolCategory)]
        public void Can_get_symbol_for_class()
        {
            var csharpCode = @"
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyClass", symbol.Name);
        }

        [TestMethod]
        [TestCategory(SymbolCategory)]
        public void Can_get_symbol_for_enum()
        {
            var csharpCode = @"
                        public enum MyEnum
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Enums.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyEnum", symbol.Name);
        }

        [TestMethod]
        [TestCategory(SymbolCategory)]
        public void Can_get_symbol_for_struct()
        {
            var csharpCode = @"
                        public struct MyStruct
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Structures.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyStruct", symbol.Name);
        }

        [TestMethod]
        [TestCategory(SymbolCategory)]
        public void Can_get_symbol_for_interface()
        {
            var csharpCode = @"
                        public interface MyInterface
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Interfaces.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyInterface", symbol.Name);
        }


        [TestMethod]
        [TestCategory(SymbolCategory)]
        public void Can_get_symbol_for_field()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myField; }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First().Fields.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("myField", symbol.Name);
        }

        [TestMethod]
        [TestCategory(SymbolCategory)]
        public void Can_get_symbol_for_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myProperty { get; } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First().Properties.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("myProperty", symbol.Name);
        }

        [TestMethod]
        [TestCategory(SymbolCategory)]
        public void Can_get_symbol_for_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First().Methods.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("myMethod", symbol.Name);
        }

        [TestMethod]
        [TestCategory(SymbolCategory)]
        public void Can_get_symbol_for_parameter()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First().Methods.First().Parameters.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("x", symbol.Name);
        }

        [TestMethod]
        [TestCategory(SymbolCategory)]
        public void Can_get_symbol_for_nestedType()
        {
            var csharpCode = @"
                        public class MyClass
                        { public class MyNestedClass {  } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((IRoslynDom)root.Classes.First().Types.First()).Symbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyNestedClass", symbol.Name);
        }
        #endregion

        #region typed symbol tests
        [TestMethod]
        [TestCategory(TypedSymbolCategory)]
        public void Can_get_typed_symbol_for_namespace()
        {
            var csharpCode = @"
                        namespace testing.Namespace1
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((RDomNamespace)root.Namespaces.First()).TypedSymbol as INamespaceSymbol ;
            Assert.AreEqual("Namespace1", symbol.Name);
            Assert.IsNotNull(symbol);
        }

        [TestMethod]
        [TestCategory(TypedSymbolCategory)]
        public void Can_get_typed_symbol_for_class()
        {
            var csharpCode = @"
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((RDomClass)root.Classes.First()).TypedSymbol as INamedTypeSymbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyClass", symbol.Name);
        }

        [TestMethod]
        [TestCategory(TypedSymbolCategory)]
        public void Can_get_typed_symbol_for_enum()
        {
            var csharpCode = @"
                        public enum MyEnum
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((RDomEnum )root.Enums.First()).TypedSymbol as INamedTypeSymbol ;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyEnum", symbol.Name);
        }

        [TestMethod]
        [TestCategory(TypedSymbolCategory)]
        public void Can_get_typed_symbol_for_struct()
        {
            var csharpCode = @"
                        public struct MyStruct
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((RDomStructure )root.Structures.First()).TypedSymbol as INamedTypeSymbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyStruct", symbol.Name);
        }

        [TestMethod]
        [TestCategory(TypedSymbolCategory)]
        public void Can_get_typed_symbol_for_interface()
        {
            var csharpCode = @"
                        public interface MyInterface
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((RDomInterface )root.Interfaces.First()).TypedSymbol as INamedTypeSymbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyInterface", symbol.Name);
        }


        [TestMethod]
       [TestCategory(TypedSymbolCategory)]
        public void Can_get_typed_symbol_for_field()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myField; }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((RDomField)root.Classes.First().Fields.First()).TypedSymbol as IFieldSymbol ;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("myField", symbol.Name);
        }

        [TestMethod]
        [TestCategory(TypedSymbolCategory)]
        public void Can_get_typed_symbol_for_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myProperty { get; } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((RDomProperty)root.Classes.First().Properties.First()).TypedSymbol as IPropertySymbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("myProperty", symbol.Name);
        }

        [TestMethod]
        [TestCategory(TypedSymbolCategory)]
        public void Can_get_typed_symbol_for_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((RDomMethod)root.Classes.First().Methods.First()).TypedSymbol as IMethodSymbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("myMethod", symbol.Name);
        }

        [TestMethod]
        [TestCategory(TypedSymbolCategory)]
        public void Can_get_typed_symbol_for_nestedType()
        {
            var csharpCode = @"
                        public class MyClass
                        { public class MyNestedClass {  } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var symbol = ((RDomClass)root.Classes.First().Types.First()).TypedSymbol as INamedTypeSymbol;
            Assert.IsNotNull(symbol);
            Assert.AreEqual("MyNestedClass", symbol.Name);
        }
        #endregion

        #region RawItem tests
        [TestMethod]
        [TestCategory(RawSyntaxCategory)]
        public void Can_get_rawSyntax_for_namespace()
        {
            var csharpCode = @"
                        namespace testing.Namespace1
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var RawItem = ((IRoslynDom)root.Namespaces.First()).RawItem;
            Assert.IsNotNull(RawItem);
        }

        [TestMethod]
        [TestCategory(RawSyntaxCategory)]
        public void Can_get_rawSyntax_for_class()
        {
            var csharpCode = @"
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var RawItem = ((IRoslynDom)root.Classes.First()).RawItem;
            Assert.IsNotNull(RawItem);
            Assert.IsTrue(RawItem is ClassDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(RawSyntaxCategory)]
        public void Can_get_rawSyntax_for_enum()
        {
            var csharpCode = @"
                        public enum MyEnum
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var RawItem = ((IRoslynDom)root.Enums.First()).RawItem;
            Assert.IsNotNull(RawItem);
            Assert.IsTrue(RawItem is EnumDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(RawSyntaxCategory)]
        public void Can_get_rawSyntax_for_struct()
        {
            var csharpCode = @"
                        public struct MyStruct
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var RawItem = ((IRoslynDom)root.Structures.First()).RawItem;
            Assert.IsNotNull(RawItem);
            Assert.IsTrue(RawItem is StructDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(RawSyntaxCategory)]
        public void Can_get_rawSyntax_for_interface()
        {
            var csharpCode = @"
                        public interface MyInterface
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var RawItem = ((IRoslynDom)root.Interfaces.First()).RawItem;
            Assert.IsNotNull(RawItem);
            Assert.IsTrue(RawItem is InterfaceDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(RawSyntaxCategory)]
        public void Can_get_rawSyntax_for_field()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myField; }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var RawItem = ((IRoslynDom)root.Classes.First().Fields.First()).RawItem;
            Assert.IsNotNull(RawItem);
            Assert.IsTrue(RawItem is VariableDeclaratorSyntax);
        }

        [TestMethod]
        [TestCategory(RawSyntaxCategory)]
        public void Can_get_rawSyntax_for_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myProperty { get; } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var RawItem = ((IRoslynDom)root.Classes.First().Properties.First()).RawItem;
            Assert.IsNotNull(RawItem);
            Assert.IsTrue(RawItem is PropertyDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(RawSyntaxCategory)]
        public void Can_get_rawSyntax_for_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var RawItem = ((IRoslynDom)root.Classes.First().Methods.First()).RawItem;
            Assert.IsNotNull(RawItem);
            Assert.IsTrue(RawItem is MethodDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(RawSyntaxCategory)]
        public void Can_get_rawSyntax_for_nestedType()
        {
            var csharpCode = @"
                        public class MyClass
                        { public class MyNestedClass {  } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var RawItem = ((IRoslynDom)root.Classes.First().Types.First()).RawItem;
            Assert.IsNotNull(RawItem);
            Assert.IsTrue(RawItem is ClassDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(RawSyntaxCategory)]
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
            Assert.Inconclusive();
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var field = root.Classes.First().Fields.First();
            var retType = field.ReturnType;
            var RawItem = retType.RawItem;
            Assert.IsNotNull(RawItem, "field");
            Assert.IsTrue(RawItem is ImmutableArray<SyntaxReference>, "field");
            var property = root.Classes.First().Properties.First();
            retType = property.ReturnType;
            RawItem = retType.RawItem;
            Assert.IsNotNull(RawItem, "property");
            Assert.IsTrue(RawItem is ImmutableArray<SyntaxReference>, "property");
            var method = root.Classes.First().Methods.First();
            retType = method.ReturnType;
            RawItem = retType.RawItem;
            Assert.IsNotNull(RawItem, "method");
            Assert.IsTrue(RawItem is ImmutableArray<SyntaxReference>, "method");
        }
        #endregion

        #region typedSyntax tests
        [TestMethod]
        [TestCategory(TypedSyntaxCategory)]
        public void Can_get_typedSyntax_for_namespace()
        {
            var csharpCode = @"
                        namespace testing.Namespace1
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var typedSyntax = ((RDomNamespace)root.Namespaces.First()).TypedSyntax;
            Assert.IsNotNull(typedSyntax);
            Assert.IsTrue(typedSyntax is NamespaceDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(TypedSyntaxCategory)]
        public void Can_get_typedSyntax_for_class()
        {
            var csharpCode = @"
                        public class MyClass
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var typedSyntax = ((RDomClass)root.Classes.First()).TypedSyntax;
            Assert.IsNotNull(typedSyntax);
            Assert.IsTrue(typedSyntax is ClassDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(TypedSyntaxCategory)]
        public void Can_get_typedSyntax_for_enum()
        {
            var csharpCode = @"
                        public enum MyEnum
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var typedSyntax = ((RDomEnum)root.Enums.First()).TypedSyntax;
            Assert.IsNotNull(typedSyntax);
            Assert.IsTrue(typedSyntax is EnumDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(TypedSyntaxCategory)]
        public void Can_get_typedSyntax_for_struct()
        {
            var csharpCode = @"
                        public struct MyStruct
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var typedSyntax = ((RDomStructure)root.Structures.First()).TypedSyntax;
            Assert.IsNotNull(typedSyntax);
            Assert.IsTrue(typedSyntax is StructDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(TypedSyntaxCategory)]
        public void Can_get_typedSyntax_for_interface()
        {
            var csharpCode = @"
                        public interface MyInterface
                            { }
                        ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var typedSyntax = ((RDomInterface)root.Interfaces.First()).TypedSyntax;
            Assert.IsNotNull(typedSyntax);
            Assert.IsTrue(typedSyntax is InterfaceDeclarationSyntax);
        }


        [TestMethod]
        [TestCategory(TypedSyntaxCategory)]
        public void Can_get_typedSyntax_for_field()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myField; }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var typedSyntax = ((RDomField)root.Classes.First().Fields.First()).TypedSyntax;
            Assert.IsNotNull(typedSyntax);
            Assert.IsTrue(typedSyntax is VariableDeclaratorSyntax );
        }

        [TestMethod]
        [TestCategory(TypedSyntaxCategory)]
        public void Can_get_typedSyntax_for_property()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myProperty { get; } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var typedSyntax = ((RDomProperty)root.Classes.First().Properties.First()).TypedSyntax;
            Assert.IsNotNull(typedSyntax);
            Assert.IsTrue(typedSyntax is PropertyDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(TypedSyntaxCategory)]
        public void Can_get_typedSyntax_for_method()
        {
            var csharpCode = @"
                        public class MyClass
                        { public int myMethod(int x) { return x; } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var typedSyntax = ((RDomMethod)root.Classes.First().Methods.First()).TypedSyntax;
            Assert.IsNotNull(typedSyntax);
            Assert.IsTrue(typedSyntax is MethodDeclarationSyntax);
        }

        [TestMethod]
        [TestCategory(TypedSyntaxCategory)]
        public void Can_get_typedSyntax_for_nestedType()
        {
            var csharpCode = @"
                        public class MyClass
                        { public class MyNestedClass {  } }";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var typedSyntax = ((RDomClass)root.Classes.First()).TypedSyntax;
            Assert.IsNotNull(typedSyntax);
            Assert.IsTrue(typedSyntax is ClassDeclarationSyntax);
        }
        #endregion

        #region stemContainer
        [TestMethod]
        [TestCategory(StemContainerCategory)]
        public void Can_get_namespaces()
        {
            var csharpCode = @"
                        namespace testing.Namespace1
                            { }

                        namespace testing.Namespace2
                            { }
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual(2, root.Namespaces.Count());
        }

        [TestMethod]
        [TestCategory(StemContainerCategory)]
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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual(3, root.Classes.Count());
        }

        [TestMethod]
        [TestCategory(StemContainerCategory)]
        public void Can_get_interfaces()
        {
            var csharpCode = @"
                        interface Interface
                            {

private string Foo{get;}}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual(1, root.Interfaces.Count());
        }

        //        public void Can_get_members()
        //        {
        //            var csharpCode = @"
        //                        class Interface
        //                            { 
        //string Foo{get;}
        //int Bar;
        //int FooBar(int cnt);
        //}
        //";
        //            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
        //            Assert.AreEqual(1, root.Interfaces.First().Members.Count());
        //        }

        [TestMethod]
        [TestCategory(StemContainerCategory)]
        public void Can_get_structures()
        {
            var csharpCode = @"
                        struct Struct1
                            { 
string Foo{get;}}
                        struct Struct1
                            { }
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual(2, root.Structures.Count());
        }

        [TestMethod]
        [TestCategory(StemContainerCategory)]
        public void Can_get_enums()
        {
            var csharpCode = @"
                        enum A
                            { }
                        enum B
                            { }
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual(2, root.Enums.Count());
        }

        [TestMethod]
        [TestCategory(StemContainerCategory)]
        public void Can_get_usings()
        {
            var csharpCode = @"
                        using A;
                        using B;
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual(2, root.Usings.Count());
        }

        [TestMethod]
        [TestCategory(StemContainerCategory)]
        public void Can_get_usings_in_namespaces()
        {
            var csharpCode = @"
namespace Foo
{
                        using A;
                        using B;
}
";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual(2, root.Namespaces.First().Usings.Count());
        }

        [TestMethod]
        [TestCategory(StemContainerCategory)]
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
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual(11, root.Types.Count());
        }
        #endregion

    }
}
