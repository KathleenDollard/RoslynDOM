﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.CSharp;
using System.Linq;
using RoslynDom.Common;
using System.Collections.Generic;

namespace RoslynDomTests
{
   [TestClass]
   public class ExpressionTests
   {
      private const string InvocationExpressionCategory = "InvocationExpression";
      private const string LambdaExpressionCategory = "LambdaExpression";

      #region invocation expression category
      [TestMethod, TestCategory(InvocationExpressionCategory)]
      public void InvocationExpression_correct_in_statement_with_no_params_and_no_generics()
      {
         var csharpCode =
       @"public class Bar
            {
                public void FooBar()
                {
                  Foo();
                }
            }";
         var exp = VerifyInvocationExpressionStatement(csharpCode, x =>
         {
            Assert.AreEqual("Foo", x.MethodName);
         });
      }

      [TestMethod, TestCategory(InvocationExpressionCategory)]
      public void InvocationExpression_correct_in_statement_with_params_and_no_generics()
      {
         var csharpCode =
       @"public class Bar
            {
                public void FooBar()
                {
                  Foo(a, b, c);
                }
            }";
         var exp = VerifyInvocationExpressionStatement(csharpCode, x =>
         {
            Assert.AreEqual("Foo", x.MethodName);
            Assert.AreEqual(3, x.Arguments.Count());
            Assert.AreEqual("a", x.Arguments.ElementAt(0).ValueExpression.InitialExpressionString );
            Assert.AreEqual("b", x.Arguments.ElementAt(1).ValueExpression.InitialExpressionString);
            Assert.AreEqual("c", x.Arguments.ElementAt(2).ValueExpression.InitialExpressionString);
            Assert.AreEqual("C#", x.Arguments.ElementAt(0).ValueExpression.InitialExpressionLanguage);
            Assert.AreEqual("C#", x.Arguments.ElementAt(1).ValueExpression.InitialExpressionLanguage);
            Assert.AreEqual("C#", x.Arguments.ElementAt(2).ValueExpression.InitialExpressionLanguage);
         });

      }

      [TestMethod, TestCategory(InvocationExpressionCategory)]
      public void InvocationExpression_correct_in_statement_with_no_params_and_generics()
      {
         var csharpCode =
      @"public class Bar
            {
                public void FooBar()
                {
                  Foo<T1, T2>();
                }
            }";
         var exp = VerifyInvocationExpressionStatement(csharpCode, x =>
         {
            Assert.AreEqual("Foo", x.MethodName);
            Assert.AreEqual(2, x.TypeArguments.Count());
            Assert.AreEqual("T1", x.TypeArguments.ElementAt(0).Name);
            Assert.AreEqual("T2", x.TypeArguments.ElementAt(1).Name);
         });
      }

      [TestMethod, TestCategory(InvocationExpressionCategory)]
      public void InvocationExpression_correct_in_statement_with_params_and_generics()
      {
         var csharpCode =
      @"public class Bar
            {
                public void FooBar()
                {
                  Foo<T1, T2>(a, b, c);
                }
            }";
         var exp = VerifyInvocationExpressionStatement(csharpCode, x =>
         {
            Assert.AreEqual("Foo", x.MethodName);
            Assert.AreEqual(3, x.Arguments.Count());
            Assert.AreEqual("a", x.Arguments.ElementAt(0).ValueExpression.InitialExpressionString);
            Assert.AreEqual("b", x.Arguments.ElementAt(1).ValueExpression.InitialExpressionString);
            Assert.AreEqual("c", x.Arguments.ElementAt(2).ValueExpression.InitialExpressionString);
            Assert.AreEqual("C#", x.Arguments.ElementAt(0).ValueExpression.InitialExpressionLanguage);
            Assert.AreEqual("C#", x.Arguments.ElementAt(1).ValueExpression.InitialExpressionLanguage);
            Assert.AreEqual("C#", x.Arguments.ElementAt(2).ValueExpression.InitialExpressionLanguage);
            Assert.AreEqual(2,    x.TypeArguments.Count());
            Assert.AreEqual("T1", x.TypeArguments.ElementAt(0).Name);
            Assert.AreEqual("T2", x.TypeArguments.ElementAt(1).Name);
         });
      }
      #endregion

      #region lambda expressions
      [TestMethod, TestCategory(InvocationExpressionCategory)]
      public void Lambda_expression_correct_in_statement_with_no_params_and_no_generics()
      {
         var csharpCode =
       @"public class Bar
            {
                public void FooBar()
                {
                   Func<int, int> y = x => x;
                }
            }";
         var exp = VerifyLambdaExpressionStatement(csharpCode, 
                     x =>  {
                              Assert.AreEqual("y", x.Left.InitialExpressionString);
                           },
                     x => {
                             Assert.AreEqual("x => x", x.InitialExpressionString);
                          });
      }

      //[TestMethod, TestCategory(InvocationExpressionCategory)]
      //public void Lambda_expression_correct_in_statement_with_no_params_and_no_generics()
      //{
      //   Func<int, int> y = x => x;
      //   Func<string, string, string> z = (x1, x2) => x1 + x2;
      //   Func<int, bool> z2 = x =>
      //   {
      //      if (x > 2) { return true; }
      //      return false;
      //   };
      //   z2(42);
      //   z("", "");
      //   var csharpCode =
      // @"public class Bar
      //      {
      //          public void FooBar()
      //          {
      //            Foo();
      //          }
      //      }";
      //   var exp = VerifyInvocationExpressionStatement(csharpCode, x =>
      //   {
      //      Assert.AreEqual("Foo", x.MethodName);
      //   });
      //}

      //[TestMethod, TestCategory(InvocationExpressionCategory)]
      //public void Lambda_expression_correct_in_statement_with_no_params_and_no_generics()
      //{
      //   Func<int, int> y = x => x;
      //   Func<string, string, string> z = (x1, x2) => x1 + x2;
      //   Func<int, bool> z2 = x =>
      //   {
      //      if (x > 2) { return true; }
      //      return false;
      //   };
      //   z2(42);
      //   z("", "");
      //   var csharpCode =
      // @"public class Bar
      //      {
      //          public void FooBar()
      //          {
      //            Foo();
      //          }
      //      }";
      //   var exp = VerifyInvocationExpressionStatement(csharpCode, x =>
      //   {
      //      Assert.AreEqual("Foo", x.MethodName);
      //   });
      //}
      #endregion

      private IInvocationExpression VerifyInvocationExpressionStatement(string csharpCode,
         Action<IHasInvocationFeatures> verify)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var output = RDom.CSharp.GetSyntaxNode(root);
         var output2 = RDom.CSharp.GetSyntaxNode(root.Copy());
         var method = root.RootClasses.First().Methods.First();
         var statement = method.Statements.First();
         Assert.IsNotNull(statement, "statement not found");
         var statementAsT = statement as IInvocationStatement;
         Assert.IsNotNull(statementAsT, "statement not correct type");
         var expressionAsT = statementAsT.Invocation as IInvocationExpression;
         Assert.IsNotNull(expressionAsT, "expression not correct type");
         verify(statementAsT);
         verify(expressionAsT);
         Assert.AreEqual(csharpCode, output.ToFullString());
         Assert.AreEqual(csharpCode, output2.ToFullString());
         return expressionAsT;
      }

      private ILambdaExpression VerifyLambdaExpressionStatement(string csharpCode,
               Action<IAssignmentStatement > verifyAssignment,
               Action<ILambdaExpression> verifyLambda)
      {
         var root = RDom.CSharp.Load(csharpCode);
         var output = RDom.CSharp.GetSyntaxNode(root);
         var output2 = RDom.CSharp.GetSyntaxNode(root.Copy());
         var method = root.RootClasses.First().Methods.First();
         var statement = method.Statements.First();
         Assert.IsNotNull(statement, "statement not found");
         var statementAsT = statement as IAssignmentStatement ;
         Assert.IsNotNull(statementAsT, "statement not correct type");
         var expressionAsT = statementAsT.Expression as ILambdaExpression;
         Assert.IsNotNull(expressionAsT, "expression not correct type");
         verifyAssignment(statementAsT);
         verifyLambda(expressionAsT);
         Assert.AreEqual(csharpCode, output.ToFullString());
         Assert.AreEqual(csharpCode, output2.ToFullString());
         return expressionAsT;
      }
   }
}
