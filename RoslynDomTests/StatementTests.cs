using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomTests
{
    [TestClass]
    public class StatementTests
    {
        private const string MethodCodeLoadCategory = "MethodCodeLoad";
        private const string PropertyCodeLoadCategory = "PropertyCodeLoad";

        #region code loading, method as example

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_misc_statements_for_method()
        {
            var csharpCode =
          @"public class Bar
            {
                public void Foo()
                {
                  if (true) {}
                  var x = "", "";
                  x = lastName + x + firstName;
                  Foo2();
                  return true;
                }
            }";
            VerifyMethodStatements<object>(csharpCode, 12, 5,
                    statements =>
                    {
                        Assert.IsInstanceOfType(statements.ElementAt(0), typeof(RDomIfStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(1), typeof(RDomDeclarationStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(2), typeof(RDomAssignmentStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(3), typeof(RDomInvocationStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(4), typeof(RDomReturnStatement));
                    });
        }


        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_declaration_statements_for_method()
        {
            var csharpCode =
          @"public class Bar
            {
                public void Foo()
                {
                  var w = "", "";
                  string x = "", "";
                  var a1 = 42;
                  int a2 = 42;
                  Int32 a3 =  42;
                  var y = new Bar(4, ""Fred"");
                  XYZ xyz = new XYZ();
                  Bar z = Bar(w, x);
                }
            }";
            VerifyMethodStatements<RDomDeclarationStatement>(csharpCode, 18, 8,
                    null);
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_if_statements_for_method()
        {
            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                    if (z == 1)
                                             {
                        var x = 42;
                    }
                    else if (z==2)
                    { var x = 43;  y = x + x; }
                    else
                    { Console.WriteLine(); }
                    if (z == 1) Console.WriteLine();
                    if (z == 2) Console.Write();
                }
            }";
            VerifyMethodStatements<RDomIfStatement>(csharpCode, 24, 3,
                    statements =>
                    {
                        var ifStatement = statements.First() as IIfStatement;
                        Assert.AreEqual(2, ifStatement.Elses.Count());
                        Assert.AreEqual(1, ifStatement.ElseIfs.Count());
                        Assert.IsNotNull(ifStatement.Else);
                        Assert.IsInstanceOfType(ifStatement.Statements.First(), typeof(RDomDeclarationStatement));
                        Assert.IsInstanceOfType(ifStatement.Elses.First().Statements.Last(), typeof(RDomAssignmentStatement));
                        Assert.IsInstanceOfType(ifStatement.Elses.Last().Statements.Last(), typeof(RDomInvocationStatement));
                    });
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_block_statements_for_method()
        {
            var csharpCode = @"
public class Bar
{
    public void Foo()
    {
        {
        var z;
        var z;
        {
        z = 43;
        z = x + y;
        z = x + y;
        z = x + y;
        }
        }
        z = Console.WriteLine();
        {}
    }
}";
            VerifyMethodStatements<object>(csharpCode, 18, 3,
                   statements =>
                   {
                       Assert.IsInstanceOfType(statements.ElementAt(0), typeof(RDomBlockStatement));
                       Assert.IsInstanceOfType(statements.ElementAt(1), typeof(RDomAssignmentStatement));
                       Assert.IsInstanceOfType(statements.ElementAt(2), typeof(RDomBlockStatement));
                       Assert.AreEqual(3, ((IBlockStatement)statements.ElementAt(0)).Statements.Count());
                       Assert.AreEqual(4, ((IBlockStatement)((IBlockStatement)statements.ElementAt(0)).Statements.Last()).Statements.Count());
                   });
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_invocation_statements_for_method()
        {
            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                    Console.WriteLine();
                    Math.Pow(4, 2);
                }
            }";
            VerifyMethodStatements<RDomInvocationStatement>(csharpCode, 7, 2,
                  null);
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_return_statements_for_method()
        {
            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                  return;
                }
                public int Foo()
                {
                  return 42;
                }
            }";
            VerifyMethodStatements<RDomReturnStatement>(csharpCode, 7, 1,
                 statements =>
                 {  });
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_while_statements_for_method()
        {
            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                    while (true)
                    {
                        Console.WriteLine();
                    }
                }
            }";
            VerifyMethodStatements<RDomWhileStatement>(csharpCode, 7, 1,
                   statements =>
                   {
                       Assert.AreEqual(2, statements.First().Children.Count());
                       Assert.AreEqual(3, statements.First().Descendants.Count());
                   });
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_do_statements_for_method()
        {
            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                    do 
                    {
                        Console.WriteLine();
                    }
                    while (true);
                }
            }";
            VerifyMethodStatements<RDomDoStatement>(csharpCode, 7, 1,
                   statements =>
                   {
                       Assert.AreEqual(2, statements.First().Children.Count());
                       Assert.AreEqual(3, statements.First().Descendants.Count());
                   });
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_for_statements_for_method()
        {

            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine(i);
                    }
                }
            }";
            VerifyMethodStatements<RDomForStatement>(csharpCode, 10, 1,
                   statements =>
                   {
                       Assert.AreEqual(4, statements.First().Children.Count());
                       Assert.AreEqual(6, statements.First().Descendants.Count());
                   });
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_for_statements_for_method_implicit_type()
        {

            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                    for (var i = 0; i < 10; i++)
                    {
                        Console.WriteLine(i);
                    }
                }
            }";
            VerifyMethodStatements<RDomForStatement>(csharpCode, 10, 1,
                   statements =>
                   {
                       Assert.AreEqual(4, statements.First().Children.Count());
                       Assert.AreEqual(6, statements.First().Descendants.Count());
                   });
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_foreach_statements_for_method()
        {

            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                    foreach (var i in new int[] { 1, 2, 3, 4, 5, 6 })
                    {
                        Console.WriteLine(i);
                    }
                }
            }";
            VerifyMethodStatements<RDomForEachStatement>(csharpCode, 8, 1,
                   statements =>
                   {
                       Assert.AreEqual(3, statements.First().Children.Count());
                       Assert.AreEqual(4, statements.First().Descendants.Count());
                   });
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_foreach_statements_for_method_explicitly_typed()
        {

            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                    foreach (int i in new int[] { 1, 2, 3, 4, 5, 6 })
                    {
                        Console.WriteLine(i);
                    }
                }
            }";
            VerifyMethodStatements<RDomForEachStatement>(csharpCode, 8, 1,
                   statements =>
                   {
                       Assert.AreEqual(3, statements.First().Children.Count());
                       Assert.AreEqual(4, statements.First().Descendants.Count());
                   });
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_try_statements_for_method()
        {
            var csharpCode = @"
            public class Bar
            {
                public void Foo()
                {
                    try
                    {
                        foreach (int i in new int[] { 1, 2, 3, 4, 5, 6 })
                        {
                            Console.WriteLine(i);
                        }
                    }
                    catch ( NotImplementedException )
                    { var a = 3; }
                    catch(DivideByZeroException ex)
                    { var b = 3; }
                    catch
                    { var c = 3; }
                    finally
                    { var d = 3; }
                }
            }";
            VerifyMethodStatements<RDomTryStatement>(csharpCode, 23, 1,
                   statements =>
                   { });
        }

        #endregion

        #region property code loading
        [TestMethod, TestCategory(PropertyCodeLoadCategory)]
        public void Can_load_return_statements_for_property()
        {
            var csharpCode = @"
            public class Bar
            {
                public string Foo
                {
                    get{
                            if (true) {}
                            var x = "","";
                            x = lastName + x + firstName;
                            Foo2();
                            return true;
                    }
                    set{
                            Foo2();
                            x = lastName + x + firstName;
                            if (true) {}
                            var x = "","";
                            return true;
                    }   
                }
            }";
            VerifyPropertyStatements(csharpCode, 25, 5,5,
                   statements =>
                   { 
                        Assert.IsInstanceOfType(statements.ElementAt(0), typeof(RDomIfStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(1), typeof(RDomDeclarationStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(2), typeof(RDomAssignmentStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(3), typeof(RDomInvocationStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(4), typeof(RDomReturnStatement));
                    },
                    statements =>
                    {
                        Assert.IsInstanceOfType(statements.ElementAt(0), typeof(RDomInvocationStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(1), typeof(RDomAssignmentStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(2), typeof(RDomIfStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(3), typeof(RDomDeclarationStatement));
                        Assert.IsInstanceOfType(statements.ElementAt(4), typeof(RDomReturnStatement));
                    });
        }
        #endregion

        private void VerifyMethodStatements<T>(string csharpCode,
                int rootDescendantCount,
                int statementCount,
                Action<IEnumerable<IStatement>> verifyDelegate)
        {
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements;
            Assert.AreEqual(statementCount, statements.Count());
            Assert.AreEqual(statementCount, statements.OfType<T>().Count()); 
            Assert.AreEqual(rootDescendantCount, root.Descendants.Count());
            if (verifyDelegate != null)
            { verifyDelegate(statements); }
            Assert.AreEqual(csharpCode, output.ToFullString());
        }

        private void VerifyPropertyStatements(string csharpCode, int rootDescendantCount, 
                int getStatementCount, int setStatementCount,
                Action<IEnumerable<IStatement>> getVerifyDelegate,
                Action<IEnumerable<IStatement>> setVerifyDelegate)
        {
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            Assert.AreEqual(rootDescendantCount, root.Descendants.Count());
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var prop = root.RootClasses.First().Properties.First();

            var getStatements = prop.GetAccessor.Statements;
            Assert.AreEqual(getStatementCount, getStatements.Count());
            if (getVerifyDelegate != null)
            { getVerifyDelegate(getStatements); }

            var setStatements = prop.SetAccessor.Statements;
            Assert.AreEqual(setStatementCount, setStatements.Count());
            if (setVerifyDelegate != null)
            { setVerifyDelegate(setStatements); }
            
            Assert.AreEqual(csharpCode, output.ToFullString());
        }
    }
}
