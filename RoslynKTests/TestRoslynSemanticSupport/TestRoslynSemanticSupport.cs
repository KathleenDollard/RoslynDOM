using KadGen.Common.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGeneration;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestRoslyn
{

    [TestClass]
    public class TestRoslynSemantic
    {
        [TestMethod]
        public void Can_create_semantic_tree()
        {
            var tree = TestRoslynSupportBase.Tree;
            Assert.IsNotNull(tree);
            List<SyntaxTree> sourceTrees = new List<SyntaxTree>();
            sourceTrees.Add(tree);

            //MetadataReference mscorlib =
            //    new MetadataFileReference(typeof(int).Assembly.Location);
            //MetadataReference roslynCompilers =
            //    new MetadataFileReference(typeof(SyntaxTree).Assembly.Location);
            //MetadataReference csCompiler =
            //    new MetadataFileReference(typeof(CSharpSyntaxTree).Assembly.Location);

            List<MetadataReference> references = new List<MetadataReference>();
            //references.Add(mscorlib);
            //references.Add(roslynCompilers);
            //references.Add(csCompiler);

            var compilation = CSharpCompilation.Create("TempCS",
                                            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                                            sourceTrees,
                                            references);
            Assert.IsNotNull(compilation);
            var model = compilation.GetSemanticModel(tree);
            Assert.IsNotNull(model);
            var classes = tree.GetClasses();
            var root = tree.GetCompilationUnitRoot();
            var classA = classes.First();
            var symbolClassAInfo = model.GetSymbolInfo(classA);
            Assert.IsNotNull(symbolClassAInfo);
            var children = root.DescendantNodes();
            var attribs = children
                .OfType<AttributeSyntax>();
            var classAttribs = attribs
               .OfType<AttributeSyntax>()
               .Where(x => x.GetName().StartsWith("ClassAttribute"));
            Assert.AreEqual(3, classAttribs.Count());
            var target = classAttribs.First().GetRealParent();
            Assert.IsInstanceOfType(target, typeof(ClassDeclarationSyntax));
            var parameterValue = classAttribs.Last().GetAttributeValue<string>();
            Assert.AreEqual("Bar", parameterValue);

        }

        [TestMethod]
        public void Can_create_class_syntax_from_code_generator()
        {
            //var symbol = new Named
            //CodeGenerator.CreateNamedTypeDeclaration()
        }
    }
}