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
   }
}
