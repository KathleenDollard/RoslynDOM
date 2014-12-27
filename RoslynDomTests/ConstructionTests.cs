using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.CSharp;
using RoslynDom;
using System.Linq;
using RoslynDom.Common;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynDomTests
{
   public class dummyTest
   {
      public class A { }
      public class B
      {
         public A A { get; set; }
         public A GetA() { return null; }
         public A[] Array { get; set; }
      }
      public class C
      {
         public B B { get; set; }
      }

      void Foo()
      {
         var c = new C();
         var x = c.B.A;
         var y = c.B.GetA();
         var z = c.B.Array[0];
         var x1 = c?.B?.A;
         var y1 = c?.B?.GetA();
         var z1 = c?.B?.Array[0];
      }
   }
   [TestClass]
   public class ConstructionTests
   {
      private string csharpCode = @"
public class Foo
{
   public int Bar()
   {
   }
}";
      [TestMethod]
      public void While_statement_can_be_constructed()
      {
         var root = RDom.CSharp.Load(csharpCode);
         var method = root.Classes.First().Methods.First();

         var expression = new RDomOtherExpression(null, "x == 1", ExpectedLanguages.CSharp, ExpressionType.Unknown);
         var whileStatement = new RDomWhileStatement(expression);
         whileStatement.HasBlock = true;
         Assert.IsNotNull(expression);
         var actual = RDom.CSharp.GetSyntaxNode(whileStatement).ToFullString();

         var expected = "while (x == 1) { }";
         Assert.AreEqual(expected, actual);
         method.StatementsAll.AddOrMove(whileStatement);
         actual = RDom.CSharp.GetSyntaxNode(method).ToFullString();
         var expectedMethod = "   public int Bar()\r\n   {\r\nwhile (x == 1) { }   }\r\n";
         Assert.AreEqual(expectedMethod, actual);
      }

      [TestMethod]
      public void ConditionalExpression_can_be_constructed()
      {
         var root = RDom.CSharp.Load(csharpCode);
         var method = root.Classes.First().Methods.First();
         var expected = "x == 1";

         var expression = new RDomOtherExpression(null, expected, ExpectedLanguages.CSharp, ExpressionType.Unknown);
         Assert.IsNotNull(expression);
         var actual = RDom.CSharp.GetSyntaxNode(expression).ToFullString();
         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void Argument_can_be_constructed()
      {
         var expected = 42;

         var expression = new RDomLiteralExpression(expected);
         var arg = new RDomArgument(expression);
         Assert.IsNotNull(expression);
         Assert.IsNotNull(arg);
         var actual = RDom.CSharp.GetSyntaxNode(arg).ToFullString();
         Assert.AreEqual(expected.ToString(), actual);
      }

      [TestMethod]
      public void Invocation_expression_can_be_parsed()
      {
         var root = RDom.CSharp.Load(csharpCode);
         var method = root.Classes.First().Methods.First();
         var expected = "Console.WriteLine(42)";
         var expression = RDom.CSharp.ParseExpression(expected);
         Assert.IsNotNull(expression);
         var actual = RDom.CSharp.GetSyntaxNode(expression).ToFullString();
         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void Invocation_expression_can_be_constructed()
      {
         var root = RDom.CSharp.Load(csharpCode);
         var method = root.Classes.First().Methods.First();
         var expected = "Console.WriteLine(42)";
         var expression = new RDomInvocationExpression("Console.WriteLine");
         var arg = new RDomArgument(new RDomLiteralExpression(42));
         expression.Arguments.AddOrMove(arg);
         Assert.IsNotNull(expression);
         var actual = RDom.CSharp.GetFormattedSourceCode(expression);
         Assert.AreEqual(expected, actual);
      }

      [TestMethod]
      public void Invocation_statement_can_be_constructed()
      {
         var root = RDom.CSharp.Load(csharpCode);
         var method = root.Classes.First().Methods.First();
         var expected = "Console.WriteLine(42)";
         var expression = new RDomInvocationExpression("Console.WriteLine");
         var arg = new RDomArgument(new RDomLiteralExpression(42));
         expression.Arguments.AddOrMove(arg);
         Assert.IsNotNull(expression);
         var actual = RDom.CSharp.GetFormattedSourceCode(expression);
         Assert.AreEqual(expected, actual);
         var statement = new RDomInvocationStatement(expression);
         actual = RDom.CSharp.GetFormattedSourceCode(statement);
         Assert.AreEqual(expected + ";", actual);
      }

   }
}
