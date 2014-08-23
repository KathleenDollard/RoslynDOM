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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(5, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomIfStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomDeclarationStatement));
            Assert.IsInstanceOfType(statements[2], typeof(RDomAssignmentStatement));
            Assert.IsInstanceOfType(statements[3], typeof(RDomInvocationStatement));
            Assert.IsInstanceOfType(statements[4], typeof(RDomReturnStatement));
            Assert.AreEqual(12, root.Descendants.Count());
            Assert.AreEqual(csharpCode , output.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(8, statements.Count());
            Assert.AreEqual(8, statements.OfType<RDomDeclarationStatement>().Count());
            Assert.AreEqual(csharpCode, output.ToFullString());
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

            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(3, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomIfStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomIfStatement));
            Assert.IsInstanceOfType(statements[2], typeof(RDomIfStatement));
            var ifStatement = statements[0] as IIfStatement;
            Assert.AreEqual(2, ifStatement.Elses.Count());
            Assert.IsInstanceOfType(ifStatement.Statements.First(), typeof(RDomDeclarationStatement));
            Assert.IsInstanceOfType(ifStatement.Elses.First().Statements.Last(), typeof(RDomAssignmentStatement));
            Assert.IsInstanceOfType((statements[0] as IIfStatement).Elses.Last().Statements.Last(), typeof(RDomInvocationStatement));

            // TODO: Solve simplification problem.
            Assert.AreEqual(csharpCode, output.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(3, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomBlockStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomAssignmentStatement));
            Assert.IsInstanceOfType(statements[2], typeof(RDomBlockStatement));
            Assert.AreEqual(3, ((IBlockStatement)statements[0]).Statements.Count());
            Assert.AreEqual(4, ((IBlockStatement)((IBlockStatement)statements[0]).Statements.Last()).Statements.Count());
            Assert.AreEqual(csharpCode, output.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(2, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomInvocationStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomInvocationStatement));
            Assert.AreEqual(csharpCode, output.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomReturnStatement));
            Assert.AreEqual(csharpCode, output.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomWhileStatement));
            Assert.AreEqual(csharpCode, output.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomDoStatement));
            Assert.AreEqual(csharpCode, output.ToFullString());

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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomForStatement));
            Assert.AreEqual(csharpCode, output.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomForStatement));
            Assert.AreEqual(csharpCode, output.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomForEachStatement));
            Assert.AreEqual(csharpCode, output.ToFullString());
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
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomForEachStatement));
            Assert.AreEqual(csharpCode, output.ToFullString());
        }

        [TestMethod, TestCategory(MethodCodeLoadCategory)]
        public void Can_load_try_statements_for_method_explicitly_typed()
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
 
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomTryStatement));
            Assert.AreEqual(csharpCode, output.ToFullString());
        }


        //try
        // break
        // continue
        // empty
        // using
        // throw


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
                            var x = "", "";
                            x = lastName + x + firstName;
                            Foo2();
                            return true;
                    }
                    set{
                            Foo2();
                            x = lastName + x + firstName;
                            if (true) {}
                            var x = "", "";
                            return true;
                    }   
                }
            }";
            var root = RDomCSharp.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharp.Factory.BuildSyntax(root);
            var property = root.RootClasses.First().Properties.First();
            var statements = property.GetAccessor.Statements.ToArray();
            Assert.AreEqual(5, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomIfStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomDeclarationStatement));
            Assert.IsInstanceOfType(statements[2], typeof(RDomAssignmentStatement));
            Assert.IsInstanceOfType(statements[3], typeof(RDomInvocationStatement));
            Assert.IsInstanceOfType(statements[4], typeof(RDomReturnStatement));
            statements = property.SetAccessor.Statements.ToArray();
            Assert.AreEqual(5, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomInvocationStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomAssignmentStatement));
            Assert.IsInstanceOfType(statements[2], typeof(RDomIfStatement));
            Assert.IsInstanceOfType(statements[3], typeof(RDomDeclarationStatement));
            Assert.IsInstanceOfType(statements[4], typeof(RDomReturnStatement));
            Assert.AreEqual(csharpCode, output.ToFullString());
        }
        #endregion
    }
}
