using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoslynDom.Common;
using RoslynDom.CSharp;

namespace RoslynDomExampleTests
{
    [TestClass]
    public class Walkthroughs
    {
        private string fileName = "Walkthrough_1_code.cs";
        private string outputFileName = "Walkthrough_1_code_test.cs";

        [TestMethod]
        public void Walkthrogh_1_Load_and_check_code()
        {
            var factory = RDomCSharp.Factory;
            var root = factory.GetRootFromFile(fileName);
            var output = factory.BuildSyntax(root).ToString();
            File.WriteAllText(outputFileName, output);
        }

        [TestMethod]
        public void Walkthrogh_2_2_Navigate_and_interrogate_code()
        {
            var root = RDomCSharp.Factory.GetRootFromFile(fileName);
            Assert.AreEqual(1, root.UsingDirectives.Count());
            Assert.AreEqual("System", root.UsingDirectives.First().Name);
            Assert.AreEqual(1, root.Namespaces.Count());
            Assert.AreEqual(1, root.RootClasses.Count());
            var methods = root.RootClasses.First().Methods.ToArray();
            Assert.AreEqual(0, methods[0].Parameters.Count());
            Assert.AreEqual(1, methods[1].Parameters.Count());
            Assert.AreEqual("dummy", methods[1].Parameters.First().Name);
        }

        [TestMethod]
        public void Walkthrogh_2_4_Ask_harder_questions()
        {
            var factory = RDomCSharp.Factory;
            var root = factory.GetRootFromFile(fileName);

            // Explore variables that have any uint type
            var uintVars = root
                .Descendants.OfType<IVariable>()
                .Where(x => x.Type.Name.StartsWith("UInt"))
                .ToArray();
            Assert.AreEqual(3, uintVars.Count());
            Assert.AreEqual("y", uintVars[0].Name);
            Assert.AreEqual("x", uintVars[1].Name);
            Assert.AreEqual("z", uintVars[2].Name);

            // Retrieve methods and properties with Uint types
            // Explore variables that have any uint type
            var uintCode = (from c in root.Descendants.OfType<IStatementContainer>()
                            from v in c.Descendants.OfType<IVariable>()
                            where v.Type.Name.StartsWith("UInt")
                            select new
                            {
                                containerName = c.Name,
                                variableName = v.Name
                            })
                            .ToArray();
            Assert.AreEqual("Foo", uintCode[0].containerName);
            Assert.AreEqual("y", uintCode[0].variableName);

            Assert.AreEqual("Foo2", uintCode[1].containerName);
            Assert.AreEqual("x", uintCode[1].variableName);

            Assert.AreEqual("get_FooBar", uintCode[2].containerName);
            Assert.AreEqual("z", uintCode[2].variableName);

        }

        [TestMethod]
        public void Walkthrogh_3_Find_implicit_variables_of_concern()
        {
            var root = RDomCSharp.Factory.GetRootFromFile(fileName);
            var candidates = FindImplicitVariablesOfConcern(root);
            var report = ReportCodeLines(candidates);
            var expected = "Walkthrough_1_code.cs(13, 16) RoslynDom.RDomDeclarationStatement : ret {String}   var ret = lastName;\r\nWalkthrough_1_code.cs(51, 16) RoslynDom.RDomDeclarationStatement : x3 {Int32}     var x3 = x2;\r\n";
            Assert.AreEqual(expected, report);
        }

        private static IEnumerable<IDeclarationStatement> FindImplicitVariablesOfConcern(IRoot root)
        {
            var implicitlyTyped = root
                            .Descendants.OfType<IDeclarationStatement>()
                            .Where(x => x.IsImplicitlyTyped);

            var instantiations = implicitlyTyped
                        .Where(x => x.Initializer.ExpressionType == ExpressionType.ObjectCreation);

            var literals = implicitlyTyped
                        .Where(x => x.Initializer.ExpressionType == ExpressionType.Literal &&
                                        (x.Type.Name == "String"
                                        || x.Type.Name == "Int32"
                                        || x.Type.Name == "DateTime")// for VB
                    );
            var candidates = implicitlyTyped.Except(instantiations).Except(literals);
            return candidates;
        }

        [TestMethod]
        public void Walkthrogh_4_Fix_implicit_variables_of_concern()
        {
            // Assumes Walkthrough_3 passes
            var root = RDomCSharp.Factory.GetRootFromFile(fileName);
            var candidates = FindImplicitVariablesOfConcern(root);
     
            foreach (var candidate  in candidates)
            {
                candidate.IsImplicitlyTyped = false;
            }
            var output = RDomCSharp.Factory.BuildSyntax(root.RootClasses.First());
            // For testing, force chhanges through secondary mechanism
            var initialCode = File.ReadAllText(fileName);
            var newCode = initialCode
                            .Replace("var ret = lastName;", "System.String ret = lastName;")
                            .Replace("var x3 = x2;", "System.Int32 x3 = x2;")
                            .SubstringAfter("Walkthrough_1_code\r\n{\r\n")
                            .SubstringBeforeLast("}")
                            ;
            Assert.AreEqual(newCode, output.ToFullString());
        }

        [TestMethod]
        public void Walkthrogh_4_Fix_non_aliased()
        {
            // Assumes Walkthrough_3 passes
            var root = RDomCSharp.Factory.GetRootFromFile(fileName);
            var candidates = FindImplicitVariablesOfConcern(root);

            foreach (var candidate in candidates)
            {
                candidate.IsImplicitlyTyped = false;
                candidate.Type.DisplayAlias = true;
            }
            var output = RDomCSharp.Factory.BuildSyntax(root.RootClasses.First());
            // For testing, force chhanges through secondary mechanism
            var initialCode = File.ReadAllText(fileName);
            var newCode = initialCode
                            .Replace("var ret = lastName;", "string ret = lastName;")
                            .Replace("var x3 = x2;", "int x3 = x2;")
                            .SubstringAfter("Walkthrough_1_code\r\n{\r\n")
                            .SubstringBeforeLast("}")
                            ;
            Assert.AreEqual(newCode, output.ToFullString());
        }

 
        private string ReportCodeLines(IEnumerable<IDom> items)
        {
            var sb = new StringBuilder();

            var lineItems = from x in items
                            select new
                            {
                                item = x,
                                fileName = GetFileName(x),
                                position = GetPosition(x),
                                code = GetNewCode(x)
                            };
            var filePathMax = lineItems.Max(x => x.fileName.Length);
            var itemMax = lineItems.Max(x => x.item.ToString().Trim().Length);
            var lineMax = lineItems.Max(x => x.position.Line.ToString().Trim().Length);
            var format = "{0, -fMax}({1,lineMax},{2,3}) {3, -itemMax}   {4}"
                        .Replace("fMax", filePathMax.ToString())
                        .Replace("itemMax", itemMax.ToString())
                        .Replace("lineMax", lineMax.ToString());
            foreach (var line in lineItems)
            {
                sb.AppendFormat(format, line.fileName, line.position.Line, line.position.Character, line.item.ToString().Trim(), line.code);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private string GetNewCode(IDom item)
        {
            var ret = new List<string>();
            return RDomCSharp.Factory.BuildFormattedSyntax(item).ToString();
        }

        private string GetOldCode(IDom item)
        {

            var node = item.RawItem as SyntaxNode;
            if (node == null)
            { return "<no syntax node>"; }
            else
            {
                return node.ToFullString();
            }
        }

        private LinePosition GetPosition(IDom item)
        {
            var node = item.RawItem as SyntaxNode;
            if (node == null)
            { return default(LinePosition); }
            else
            {
                var location = node.GetLocation();
                var linePos = location.GetLineSpan().StartLinePosition;
                return linePos;
            }
        }

        private string GetFileName(IDom item)
        {
            var root = item.Ancestors.OfType<IRoot>().FirstOrDefault();
            if (root != null)
            { return root.FilePath; }
            else
            {
                var top = item.Ancestors.Last();
                var node = top as SyntaxNode;
                if (node == null)
                { return "<no file name>"; }
                else
                { return node.SyntaxTree.FilePath; }
            }
        }
    }
}
