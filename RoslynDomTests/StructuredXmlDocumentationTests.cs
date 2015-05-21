using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.CSharp;
using RoslynDom.Common;
using RoslynDom;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslynDomTests
{
   public class Foo
   {
      /// <summary>
      /// This is a test
      ///
      /// more test
      /// </summary>
      /// <param name=""dummy"">With a dummy parameter</param>
      public void Foo3(string dummy)
      {
         Console.WriteLine(42);
      }
   }

   [TestClass]
   public class StructuredXmlDocumentationTests
   {
      private string csharpCode =
@"public class Foo
{
   /// <summary>
   /// This is a test
   /// </summary>
   /// <param name=""dummy"">
   /// With a dummy parameter
   /// </param>
   public void Foo3(string dummy)
   {
      Console.WriteLine(42);
   }
}";

      [TestMethod]
      public void Can_load_and_retrieve_structured_comments()
      {

         VerifyStructuredComments(csharpCode, x => x.Classes.First().Methods.First(), "\r\nThis is a test\r\n");
      }


      [TestMethod]
      public void Can_load_and_retrieve_multiline_structured_comments()
      {
         var csharpCode =
@"public class Foo
{
   /// <summary>
   /// This is a test and another test and another
   /// and another and another
   /// </summary>
   /// <param name=""dummy"">
   /// With a dummy parameter
   /// </param>
   public void Foo3(string dummy)
   {
      Console.WriteLine(42);
   }
}";
         VerifyStructuredComments(csharpCode, x => x.Classes.First().Methods.First(),
            "\r\nThis is a test and another test and another\r\nand another and another\r\n");
      }

      public void VerifyStructuredComments(string csharpCode,
                  Func<IRoot, IHasStructuredDocumentation> makeItem,
                  string description)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var root2 = root.Copy();
         var item = makeItem(root);
         var item2 = makeItem(root2);
         Assert.IsNotNull(item2);
         Assert.AreEqual(description, item.Description);
         Assert.AreEqual(description, item2.Description);
         Assert.IsTrue(item.SameIntent(item2));
         var actual1 = RDom.CSharp.GetSyntaxNode(root).ToFullString();
         var actual2 = RDom.CSharp.GetSyntaxNode(root2).ToFullString();
         Assert.AreEqual(csharpCode, actual1);
         Assert.AreEqual(csharpCode, actual2);


      }


      private string csharpCode2 =
@"public class Foo
{
   /// <summary>
   /// This is a test
   ///
   /// more test
   /// </summary>
   /// <param name=""dummy"">
   /// With a dummy parameter
   /// </param>
   public void Foo3(string dummy)
   {
      Console.WriteLine(42);
   }
}";

      [TestMethod]
      public void Parse_trivia()
      {
         var test = @"   /// <summary>
   /// This is a test
   ///
   /// more test
   /// </summary>
   /// <param name=""dummy"">
   /// With a dummy parameter
   /// </param>";
         var temp = SyntaxFactory.ParseLeadingTrivia(test);

}

      [TestMethod]
      public void Can_create_StructuredDocumentat_from_structured_comments()
      {
         VerifyStructuredComments2(csharpCode2,
            x => x.Classes.First().Methods.First(),
            CreateExpected("summary", "\r\nThis is a test\r\n\r\nmore test\r\n"),
            CreateExpected("param", "\r\nWith a dummy parameter\r\n", Tuple.Create("name", "dummy")));
      }

      private Tuple<string, string, Tuple<string, string>[]> CreateExpected(
               string name, string text, params Tuple<string, string>[] attributePairs)
      {
         return Tuple.Create(name, text, attributePairs);
      }

      public void VerifyStructuredComments2(string csharpCode,
             Func<IRoot, IHasStructuredDocumentation> makeItem,
             params Tuple<string, string, Tuple<string, string>[]>[] expectedElements)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var root2 = root.Copy();
         var item = makeItem(root).StructuredDocumentation  as RDomStructuredDocumentation;
         var item2 = makeItem(root2).StructuredDocumentation  as RDomStructuredDocumentation;
         Assert.IsNotNull(item2);

         Assert.AreEqual(expectedElements.Count(), item.Elements.Count());
         for (int i = 0; i < expectedElements.Count(); i++)
         {
            VerifyStructuredCommentElement(expectedElements[i], item.Elements.ElementAt(i));
         }

         Assert.IsTrue(item.SameIntent(item2));
         var actual1 = RDom.CSharp.GetSyntaxNode(root).ToFullString();
         var actual2 = RDom.CSharp.GetSyntaxNode(root2).ToFullString();
         Assert.AreEqual(csharpCode, actual1);
         Assert.AreEqual(csharpCode, actual2);


      }

      private void VerifyStructuredCommentElement(
         Tuple<string, string, Tuple<string, string>[]> expected,
         IStructuredDocumentationElement element)
      {
         Assert.AreEqual(expected.Item1, element.Name);
         Assert.AreEqual(expected.Item2, element.Text);
         var attributePairs = expected.Item3;
         for (int i = 0; i < attributePairs.Length; i++)
         {
            var expectedAttribute = attributePairs[i];
            var actualAttribute = element.Attributes.ElementAt(i);
            Assert.AreEqual(expectedAttribute.Item1, actualAttribute.Name);
            Assert.AreEqual(expectedAttribute.Item2, actualAttribute.AttributeValues.First().Value);
         }
      }

      private static void VerifyStructuredElement(IStructuredDocumentationElement element,
         string name, string text,
         params Tuple<string, string>[] attributePairs)
      {
         Assert.AreEqual(name, element.Name);
         Assert.AreEqual(text, element.Text);
         for (int i = 0; i < attributePairs.Length; i++)
         {
            var expected = attributePairs[i];
            var actual = element.Attributes.ElementAt(i);
            Assert.AreEqual(expected.Item1, actual.Name);
            Assert.AreEqual(expected.Item2, actual.AttributeValues.First().Value);
         }
      }

   }
}
