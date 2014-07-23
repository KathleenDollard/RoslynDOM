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
            }           
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(5, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomIfStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomDeclarationStatement));
            Assert.IsInstanceOfType(statements[2], typeof(RDomAssignmentStatement));
            Assert.IsInstanceOfType(statements[3], typeof(RDomInvocationStatement));
            Assert.IsInstanceOfType(statements[4], typeof(RDomReturnStatement));
            Assert.AreEqual(12, root.Descendants.Count());
            var expectedString = "public class Bar\r\n{\r\n    public Void Foo()\r\n    {\r\n        if (true)\r\n        {\r\n        }\r\n\r\n        var x = \", \";\r\n        x = lastName + x + firstName;\r\n        Foo2();\r\n        return true;\r\n    }\r\n}";
            Assert.AreEqual(expectedString, output.ToString());
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
                  var y = new Bar(4, ""Fred"");
                  XYZ xyz = new XYZ();
                  Bar z = Bar(w, x);
                }
            }           
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(5, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomDeclarationStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomDeclarationStatement));
            Assert.IsInstanceOfType(statements[2], typeof(RDomDeclarationStatement));
            Assert.IsInstanceOfType(statements[3], typeof(RDomDeclarationStatement));
            Assert.IsInstanceOfType(statements[4], typeof(RDomDeclarationStatement));
            // TODO: Solve simplification problem.
            var actual = RoslynCSharpUtilities.Simplify(output);
            var expectedString = "public class Bar\r\n{\r\n    public Void Foo()\r\n    {\r\n        var w = \", \";\r\n        String x = \", \";\r\n        var y = new Bar(4, \"Fred\");\r\n        XYZ xyz = new XYZ();\r\n        Bar z = Bar(w, x);\r\n    }\r\n}";
            Assert.AreEqual(expectedString, actual);
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
            }           
            ";

            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
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
            var actual = RoslynCSharpUtilities.Simplify(output);
            var expectedString = "public class Bar\r\n{\r\n    public Void Foo()\r\n    {\r\n        if (z == 1)\r\n        {\r\n            var x = 42;\r\n        }\r\n        else if (z == 2)\r\n        {\r\n            var x = 43;\r\n            y = x + x;\r\n        }\r\n        else\r\n        {\r\n            Console.WriteLine();\r\n        }\r\n\r\n        if (z == 1)\r\n            Console.WriteLine();\r\n        if (z == 2)\r\n            Console.Write();\r\n    }\r\n}";
            Assert.AreEqual(expectedString, actual);
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
            }           
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(3, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomBlockStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomAssignmentStatement));
            Assert.IsInstanceOfType(statements[2], typeof(RDomBlockStatement));
            Assert.AreEqual(3, ((IBlockStatement)statements[0]).Statements.Count());
            Assert.AreEqual(4, ((IBlockStatement)((IBlockStatement)statements[0]).Statements.Last()).Statements.Count());
            // TODO: Solve simplification problem.
            var actual = RoslynCSharpUtilities.Simplify(output);
            var expectedString = "public class Bar\r\n{\r\n    public Void Foo()\r\n    {\r\n        {\r\n            var z;\r\n            var z;\r\n            {\r\n                z = 43;\r\n                z = x + y;\r\n                z = x + y;\r\n                z = x + y;\r\n            }\r\n        }\r\n\r\n        z = Console.WriteLine();\r\n        {\r\n        }\r\n    }\r\n}";
            Assert.AreEqual(expectedString, actual);
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
            }           
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(2, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomInvocationStatement));
            Assert.IsInstanceOfType(statements[1], typeof(RDomInvocationStatement));
            // TODO: Solve simplification problem.
            var actual = RoslynCSharpUtilities.Simplify(output);
            var expectedString = "public class Bar\r\n{\r\n    public Void Foo()\r\n    {\r\n        Console.WriteLine();\r\n        Math.Pow(4, 2);\r\n    }\r\n}";
            Assert.AreEqual(expectedString, actual);
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
            }           
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomReturnStatement));
            // TODO: Solve simplification problem.
            var actual = RoslynCSharpUtilities.Simplify(output);
            var expectedString = "public class Bar\r\n{\r\n    public Void Foo()\r\n    {\r\n        return;\r\n    }\r\n\r\n    public Int32 Foo()\r\n    {\r\n        return 42;\r\n    }\r\n}";
            Assert.AreEqual(expectedString, actual);
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
            }           
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomWhileStatement));
            var actual = output.ToString();
            var expectedString = "public class Bar\r\n{\r\n    public Void Foo()\r\n    {\r\n        while (true)\r\n        {\r\n            Console.WriteLine();\r\n        }\r\n    }\r\n}";
            Assert.AreEqual(expectedString, actual);
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
                    while (true)
                }
            }           
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomDoStatement));
            var actual = output.ToString();
            var expectedString = "public class Bar\r\n{\r\n    public Void Foo()\r\n    {\r\n        do\r\n        {\r\n            Console.WriteLine();\r\n        }\r\n        while (true);\r\n    }\r\n}";
            Assert.AreEqual(expectedString, actual);
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
            }           
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomForStatement));
            var actual = output.ToString();
            var expectedString = "public class Bar\r\n{\r\n    public Void Foo()\r\n    {\r\n        for (Int32 i = 0; i < 10; i++)\r\n        {\r\n            Console.WriteLine(i);\r\n        }\r\n    }\r\n}";
            Assert.AreEqual(expectedString, actual);
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
            }           
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
            var method = root.RootClasses.First().Methods.First();
            var statements = method.Statements.ToArray();
            Assert.AreEqual(1, statements.Count());
            Assert.IsInstanceOfType(statements[0], typeof(RDomForEachStatement));
            var actual = output.ToString();
            var expectedString = "public class Bar\r\n{\r\n    public Void Foo()\r\n    {\r\n        foreach (var i in new int[] { 1, 2, 3, 4, 5, 6 })\r\n        {\r\n            Console.WriteLine(i);\r\n        }\r\n    }\r\n}";
            Assert.AreEqual(expectedString, actual);
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
            }           
            ";
            var root = RDomCSharpFactory.Factory.GetRootFromString(csharpCode);
            var output = RDomCSharpFactory.Factory.BuildSyntax(root);
            var property = root.RootClasses.First().Properties .First();
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
            var expectedString = "public class Bar\r\n{\r\n    public String Foo\r\n    {\r\n        get\r\n        {\r\n            if (true)\r\n            {\r\n            }\r\n\r\n            var x = \", \";\r\n            x = lastName + x + firstName;\r\n            Foo2();\r\n            return true;\r\n        }\r\n\r\n        set\r\n        {\r\n            Foo2();\r\n            x = lastName + x + firstName;\r\n            if (true)\r\n            {\r\n            }\r\n\r\n            var x = \", \";\r\n            return true;\r\n        }\r\n    }\r\n}";
            Assert.AreEqual(expectedString, output.ToString());
        }
        #endregion
    }
}
