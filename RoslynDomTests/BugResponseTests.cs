using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.CSharp;
using RoslynDom.Common;

namespace RoslynDomTests
{
   [TestClass]
   public class BugResponseTests
   {
      private const string BugResponseCategory = "BugResponse";

      [TestMethod, TestCategory(BugResponseCategory)] // #46
      public void Can_parse_for_property_bug_46()
      {
         var csharpCode = @"
                        public class MyClass
                        { 
                            protected int _flotDialogue
                            {
                                get { return MyStaticClass1._flotDialogue; }
                                set { MyStaticClass1._flotDialogue = value; }
                            }
                        }";
         var root = RDom.CSharp.Load(csharpCode);
         var symbol = ((RDomProperty)root.Classes.First().Properties.First()).TypedSymbol as IPropertySymbol;
         Assert.IsNotNull(symbol);
         Assert.AreEqual("_flotDialogue", symbol.Name);
      }

      [TestMethod, TestCategory(BugResponseCategory)]
      public void Can_rebuild_method_with_structured_documentation()
      {
         var csharpCode =
@"   public class Foo
     {

        /// <summary>
        /// This is a test
        /// </summary>
        /// <param name=""dummy"">With a dummy parameter</param>
        public void Foo3(string dummy)
        {
            var x2 = 3;
            var x3 = x2;
        }
     }";
         var root = RDom.CSharp.Load(csharpCode);
         var output = RDom.CSharp.GetSyntaxNode(root);
         Assert.AreEqual(csharpCode, output.ToFullString());
      }

      [TestMethod, TestCategory(BugResponseCategory)]
      public void Can_correctly_match_region_start_end()
      {
         var csharpCode =
@"  using ExpansionFirst.Support;

namespace ExpansionFirstTemplateTests
{
namespace NotifyPropertyChanged
{
   #region  _xf_MakeFileForEach(Over=""Meta.Classes"", VarName=""class_"") 
   using System.ComponentModel;

   //[[// There will almost always be just one class in the file. This also sets loop var name ]]
   #region [[ _xf_ForEach(LoopOver=""Meta.Classes"", VarName=""Class"") ]]
   namespace _xf_Class_namespaceName
   {
      //[[// Assumes that the source is a clean and disposable listing of properties that are candidates ]]
      //[[// for property changed and that there is a partial class with custom code and a base class ]]
      //[[// that includes a SetProperty method   ]]
      [_xf_.OutputAsPartial()]
      public sealed partial class _xf_Class_dot_Name : INotifyPropertyChanged
      {
         public event PropertyChangedEventHandler PropertyChanged;

         #region [[_xf_ForEach(LoopOver=""_xf_Class_dot_Properties"", LoopVar=""Property"") ]]
         #region Your properties - just included so you could see normal regions still work

         private _xf_Property_dot_Type _xf_Camel_Property_dot_Name;
         [_xf_.OutputXmlComments]
         [_xf_.OutputAttributes]
         public _xf_Property_dot_Type _xf_Property_dot_Name
         {
            get { return _xf_Camel_Property_dot_Name; }
            set { SetProperty(ref _xf_Camel_Property_dot_Name, value); }
         }
         #endregion
         #endregion
      }
   }

   #endregion

#endregion
}
}";
         var root = RDom.CSharp.Load(csharpCode);
         var regions = root.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(4, regions.Count());
         Assert.IsNotNull(regions[0].BlockEnd);
         Assert.IsNotNull(regions[1].BlockEnd);
         Assert.IsNotNull(regions[2].BlockEnd);
         Assert.IsNotNull(regions[3].BlockEnd);
         var root2 = root.Copy();
         var regions2 = root2.Descendants.OfType<IDetailBlockStart>().ToArray();
         Assert.AreEqual(4, regions2.Count());
         Assert.IsNotNull(regions2[0].BlockEnd);
         Assert.IsNotNull(regions2[1].BlockEnd);
         Assert.IsNotNull(regions2[2].BlockEnd);
         Assert.IsNotNull(regions2[3].BlockEnd);
         Assert.AreEqual(5, regions2[0].BlockContents.NoWhitespace().Count());
         Assert.AreEqual(1, regions2[1].BlockContents.NoWhitespace().Count());
         Assert.AreEqual(4, regions2[2].BlockContents.NoWhitespace().Count());
         Assert.AreEqual(2, regions2[3].BlockContents.NoWhitespace().Count());
      }

      [TestMethod, TestCategory(BugResponseCategory)]
      public void Is_fixed_vertical_whitespace_issue()
      {
         var csharpCode = @"
using System;
public class ContractNamespaceAttribute : Attribute
{
   public ContractNamespaceAttribute(string text)
   {
   }

   public Type MyType {get; set;}
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var output = RDom.CSharp.GetSyntaxNode(root);
         var actual = output.ToFullString();
         Assert.AreEqual(csharpCode, actual);
      }

      [TestMethod, TestCategory(BugResponseCategory)]
      public void Is_fixed_vertical_whitespace_issue_2()
      {
         var csharpCode = @"
using System;

namespace Test
{}";
         var root = RDom.CSharp.Load(csharpCode);
         var output = RDom.CSharp.GetSyntaxNode(root);
         var actual = output.ToFullString();
         Assert.AreEqual(csharpCode, actual);
      }

      [TestMethod, TestCategory(BugResponseCategory)]
      public void Is_fixed_duplicate_comment_issue()
      {
         var csharpCode = @"
           public class Bar
            {
               public string Foo()
               {

                  var ret = lastName;

                  // Comment and whitespace
                  var xx = new String('a', 4);
               }
            }";
         var root = RDom.CSharp.Load(csharpCode);
         var output = RDom.CSharp.GetSyntaxNode(root);
         var actual = output.ToFullString();
         Assert.AreEqual(csharpCode, actual);
      }

      [TestMethod, TestCategory(BugResponseCategory)]
      public void Is_fixed_duplicate_comment_issue_2()
      {
         Assert.Inconclusive("The problem here is that the comment appears in trailing trivia");
         var csharpCode = @"
           public class Bar
            {
               public string Foo()
               {

                  var ret = lastName;

                  // Comment and whitespace
               }
            }";
         var root = RDom.CSharp.Load(csharpCode);
         var output = RDom.CSharp.GetSyntaxNode(root);
         var actual = output.ToFullString();
         Assert.AreEqual(csharpCode, actual);
      }

      [TestMethod, TestCategory(BugResponseCategory)]
      public void Is_fixed_region_issue_2()
      {
         var csharpCode = @"
           public class Bar
            {
               public string Foo()
               {

                  var ret2 = lastName;
                   #region my region
                  var ret3 = lastName;
                  #endregion
                  var ret4 = lastName;
               }
            }";
         var root = RDom.CSharp.Load(csharpCode);
         var output = RDom.CSharp.GetSyntaxNode(root);
         var actual = output.ToFullString();
         Assert.AreEqual(csharpCode, actual);
      }


      [TestMethod, TestCategory(BugResponseCategory)]
      public void Is_fixed_missing_usings()
      {
         var csharpCode = @"
 namespace ExpansionFirstTemplateTests
{
   namespace NotifyPropertyChanged
   {

      using System.ComponentModel;
      using System;
      #region _xf_MakeFileForEach(Over=asdf) 

      namespace _xf_Class_namespaceName
      {}
      #endregion
   }
}
";
         var root = RDom.CSharp.Load(csharpCode);
         var output = RDom.CSharp.GetSyntaxNode(root);
         var actual = output.ToFullString();
         Assert.AreEqual(csharpCode, actual);
      }

      [TestMethod, TestCategory(BugResponseCategory)]
      public void Load_Code_First_for_analyzer()
      {
         var csharpCode = @"using CodeFirstAnalyzer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
         using Microsoft.CodeAnalysis;
         using Microsoft.CodeAnalysis.CSharp;

namespace KathleensAnalyzer
   {
      public class IfElseBraceDiagnosticCodeFirst : DiagnosticAndCodeFixBase
      {
         public IfElseBraceDiagnosticCodeFirst()
         {
            base.
            Id = ""KADGEN1001"";
            Description = ""Needs braces"";
            MessageFormat = ""{0} needs braces"";
            Category = ""Style"";
            AddAnalyzer<IfStatementSyntax>(
               condition: x => !x.Statement.IsKind(SyntaxKind.Block),
               getLocation: x => x.IfKeyword.GetLocation(),
               messageArgs: ""if statement"");
            AddCodeFix<IfStatementSyntax>(
               makeNewNode: x => x.WithStatement(
                        SyntaxFactory.Block(x.Statement)));
            AddAnalyzer<ElseClauseSyntax>(
              condition: x => !x.Statement.IsKind(SyntaxKind.Block)
                              && !x.Statement.IsKind(SyntaxKind.IfStatement),
              getLocation: x => x.ElseKeyword.GetLocation(),
              messageArgs: ""else statement"");
            AddCodeFix<ElseClauseSyntax>(
              makeNewNode: GetNewElseNode);
         }

         private SyntaxNode GetNewElseNode(ElseClauseSyntax elseClause)
         {
            return elseClause.WithStatement(SyntaxFactory.Block(elseClause.Statement));
         }

         // Methods used by the lambdas in the constructor appear here, or 
         // methods that can be directly used as delegates,
      }

      [DiagnosticAndCodeFix]
      public class DiagnosticAndCodeFixBase
      {
         public string Id { get; set; }
         public string Description { get; set; }
         public string MessageFormat { get; set; }
         public string Category { get; set; }
         protected static Diagnostic Report(Location location, params string[] messageArgs) { return null; }
         protected static Diagnostic Report(SyntaxToken token, params string[] messageArgs) { return null; }

         public void AddAnalyzer<TSyntaxNode>(Func<TSyntaxNode, bool> condition, Func<TSyntaxNode, Location> getLocation,
             params string[] messageArgs)
         { }
         public void AddCodeFix<TSyntaxNode>(Func<TSyntaxNode, SyntaxNode> makeNewNode, bool skipFormatting = false, params string[] messageArgs)
         { }
         }
      }
";
         var root = RDom.CSharp.Load(csharpCode);
         var c1 = root.RootClasses.First();
         var baseType = c1.BaseType;
         Assert.AreEqual("DiagnosticAndCodeFixBase", baseType.Type.Name);
         var output = RDom.CSharp.GetSyntaxNode(root);
         var actual = output.ToFullString();
         //Assert.AreEqual(csharpCode, actual);
      }

      [TestMethod]
      public void Issue_88_generated_class_results_in_uncompilable_code()
      {
         var csharpCode = @"using RoslynDom.Common;
namespace Company.Core
   {
      public class className
      {
      }
   }";

         IRoot root = RDom.CSharp.Load(csharpCode);
         var firstClass = root.RootClasses.First();
         firstClass.Name = "Product";

         var backerField = new RDomField(name: "mRevisions", returnTypeName: "StringField");
         firstClass.AddOrMoveMember(backerField);
         var property = new RDomProperty(name: "Revisions", propertyTypeName: "string", declaredAccessModifier: AccessModifier.Public, isVirtual: true);
         firstClass.AddOrMoveMember(property);

         var formatted = RDom.CSharp.GetFormattedSourceCode(root);
         var expected = "using RoslynDom.Common;\r\nnamespace Company.Core\r\n{\r\n    public class Product\r\n    {\r\n        private StringField mRevisions;\r\n        public virtual string Revisions { private get; private set; }\r\n    }\r\n}";
         Assert.AreEqual(expected, formatted);

      }
   }
}
