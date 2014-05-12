using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KadGen.Common.Roslyn;
using KadGen.KadEventSource;
using System.Linq;
using System.Diagnostics.Tracing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Kad.Common.Roslyn;
using System.Collections.Generic;
using KadGen.Common.Roslyn.ExpansionParser;
using KadGen.Common.Roslyn.ExpansionBuilder;

//MetadataContext

// find region end
// region foreach to structured trivia

// region assign to structured trivia


//EvaluateForEach
//ReplaceIdentifiers
//ReplaceInComments
//EvaluateSpecial

namespace TestRoslyn
{

    //   Evaluate - 
    //Copy down to loop atribute
    //   Cycles through EventSources
    //   Set metadata
    //   Copy with replaced identifiers
    //   Copy children looking for next special attribute

    [TestClass]
    public class TestExpansionParser
    {
        private string testDirective = "IncludesInterface";

        private string treeText
        { get { return @"
"; } }
        private string treeTextTemp
        {
            get
            {
                return @"
using KadGen.KadEventSource;
using KadGen.Common.Roslyn;

#region [[_kad_FileForEach(LoopOver=""Meta"", LoopVarName=""_class_"")]] 
namespace kad_Template
    {
        using System;
        using System.Collections.Generic;
        using System.Diagnostics.Tracing;

        namespace _namespaceName
        {
            #region [[_kad_ForEach(LoopOver = ""meta.Classes"", LoopVarName = ""_class_"")]]
            public partial class _class_ClassName
            {
                // Private constructor blocks direct instantiation of class
                private _class_className() { }
            }
            #endregion
        }
    }
#endregion
";

            }
        }

        private IEnumerable<KadEventSource> eventSourceWithInterface
        {
            get
            {
                var metaString = @"
using KadGen.KadMan;
using TestRoslyn;

namespace KadGen.ETWPreMan
    {
        [Name(""KadGen-EtwSamples2-NormalEventSource"")]
        public class NormalEventSource2 : TestRoslyn.INormalLogger
        {
            public void foo() { }
            public void MyFirstActionStart(string param1, int param2) { }
            public void MyFirstActionStop() { }
        }
    }
";
                var parser = new KadGen.Common.Roslyn.ParserForMetadata<KadEventSource>();
                return parser.Parse(metaString);
            }
        }

        private IEnumerable<KadEventSource> eventSourceWithoutInterface
        {
            get
            {
                var metaString = @"using KadGen.KadMan;
using TestRoslyn;

namespace KadGen.ETWPreMan
    {
        [Name(""KadGen-EtwSamples2-NormalEventSource"")]
        public class NormalEventSource2 
        {
            public void foo() { }
            public void MyFirstActionStart(string param1, int param2) { }
            public void MyFirstActionStop() { }
        }
    }
";
                var parser = new KadGen.Common.Roslyn.ParserForMetadata<KadEventSource>();
                return parser.Parse(metaString);
            }
        }

        [TestMethod]
        public void Can_create_namespace_loop()
        {
            //var parser = new KadGen.Common.Roslyn.ExpansionParser.Parser2();
            //var expansionSets = parser.GetExpansionSets(treeTextTemp);
            Assert.Inconclusive();
        }

        private void TestSpace()
        {
            SyntaxNode node;
            SyntaxToken token;
            SyntaxTrivia  trivia;
            MemberDeclarationSyntax member;
            ClassDeclarationSyntax cl;
            NamespaceDeclarationSyntax ns;
            CompilationUnitSyntax root;

        }


        [TestMethod]
        public void Should_create_metadata_context_with_correct_contents()
        {
            var source = @"
#region test
    namespace kad_Template
    {
#region [[_kad_ForEach(LoopOver=""Meta"", LoopVarName=""_class_"")]] 
        namespace _namespaceName
        {
            #region [[_kad_ForEach(LoopOver = ""meta.Classes"", LoopVarName = ""_class_"")]]
            public partial class _class_ClassName
            {          }
            #endregion
        }
#endregion
    }
#endregion
";
            var parser = new Parser2();
            CompilationUnitSyntax  unit = parser.GetUpdatedTree(source);
            var ns1 =(NamespaceDeclarationSyntax) unit.Members[0];
            var ns2 = (NamespaceDeclarationSyntax)ns1.Members[0];
            var c1 = (ClassDeclarationSyntax) ns2.Members[0];
            var annot1 = ns1.GetAnnotations(ExpansionConstants.ExpansionSystemName  );
            var annot2 = ns2.GetAnnotations(ExpansionConstants.ExpansionSystemName);
            var annot3 = c1.GetAnnotations(ExpansionConstants.ExpansionSystemName);
            Assert.AreEqual(0, annot1.Count());
            Assert.AreEqual(1, annot2.Count());
            Assert.AreEqual(1, annot3.Count());
            var data1 = annot2.First().Data;
            var data2= annot3.First().Data;
            var nodes = unit.GetAnnotatedNodesAndTokens(ExpansionConstants.ExpansionSystemName);
            var members1 = ns1.Members;
            var members2 = ns2.Members;
            var members3 = c1.Members;
        }

        [TestMethod]
        public void Should_create_correct_output_from_template()
        {
            var source = @"
#region test
    namespace kad_Template
    {
#region [[_kad_ForEach(LoopOver=""Meta"", LoopVarName=""_class_"")]] 
        namespace _namespaceName
        {
            #region [[_kad_ForEach(LoopOver = ""Meta.Classes"", LoopVarName = ""_class_"")]]
            public partial class _class_ClassName
            {          }
            #endregion
        }
#endregion
    }
#endregion
";
            var parser = new Parser2();
            CompilationUnitSyntax unit = parser.GetUpdatedTree(source);
            var newTree = SyntaxFactory.SyntaxTree(unit);

            // dummy if declarations
            string[] ifDeclarations = new string[] { };
            var expansionSet = new ExpansionSet(newTree, ifDeclarations);

            var metadataDefinition = new MetadataDefinition("Meta", eventSourceWithoutInterface);
            var metadataContext = new MetadataContext("Root", null, null, metadataDefinition);

            var builder = new Builder4();
            builder.Build(expansionSet, metadataContext);

            Assert.Inconclusive();
        }

 

        //[TestMethod]
        //public void Should_create_metadata_context_with_correct_childContexts()
        //{
        //    Assert.Inconclusive();

        //}

        //[TestMethod]
        //public void Should_create_metadata_context_with_correct_metadataDefinitions()
        //{
        //    Assert.Inconclusive();
        //}

        #region New RoslynSyntaxSupport Tests. Should move later
        [TestMethod]
        public void Should_get_regions()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region test
#region [[_kad_FileForEach(LoopOver=""Meta"", LoopVarName=""_class_"")]] 
    namespace kad_Template
    {
        namespace _namespaceName
        {
            #region [[_kad_ForEach(LoopOver = ""meta.Classes"", LoopVarName = ""_class_"")]]
            public partial class _class_ClassName
            {          }
            #endregion
        }
    }
#endregion
#endregion
");
            var regions = tree.GetAllRegionDirectives();
            Assert.AreEqual(3, regions.Count());
        }

        [TestMethod]
        public void Should_get_region_text()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region test
#region [[_kad_FileForEach(LoopOver=""Meta"", LoopVarName=""_class_"")]] 
  
#endregion
#endregion
");
            var regions = tree.GetAllRegionDirectives();
            Assert.AreEqual("test", regions.First().GetName());
            Assert.AreEqual(@"[[_kad_FileForEach(LoopOver=""Meta"", LoopVarName=""_class_"")]]", regions.Last().GetName());
        }

        [TestMethod]
        public void Should_get_regions_matching_predicate()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region test
#region [[_kad_FileForEach(LoopOver=""Meta"", LoopVarName=""_class_"")]] 
    namespace kad_Template
    {
        namespace _namespaceName
        {
            #region [[_kad_ForEach(LoopOver = ""meta.Classes"", LoopVarName = ""_class_"")]]
            public partial class _class_ClassName
            {          }
            #endregion
        }
    }
#endregion
#endregion
");
            var regions = tree.GetAllRegionDirectives(x => x.GetName().Trim().StartsWith("[["));
            Assert.AreEqual(2, regions.Count());
        }

        [TestMethod]
        public void Should_get_regions_matching_prefix()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region test
#region [[_kad_FileForEach(LoopOver=""Meta"", LoopVarName=""_class_"")]] 
    namespace kad_Template
    {
        namespace _namespaceName
        {
            #region [[_kad_ForEach(LoopOver = ""meta.Classes"", LoopVarName = ""_class_"")]]
            public partial class _class_ClassName
            {          }
            #endregion
        }
    }
#endregion
#endregion
");
            var regions = tree.GetAllRegionDirectives("[[");
            Assert.AreEqual(2, regions.Count());
        }

        [TestMethod]
        public void Should_return_empty_when_no_regions()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
    namespace kad_Template
    {
        namespace _namespaceName
        {
            public partial class _class_ClassName
            {          }
        }
    }
");
            var regions = tree.GetAllRegionDirectives(x => x.GetName().Trim().StartsWith("[["));
            Assert.AreEqual(0, regions.Count());
        }

        [TestMethod]
        public void Should_set_and_see_annotation()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region Whatever
#region Whenever
       namespace _namespaceName
        {
            #region [[_kad_ForEach(LoopOver = ""meta.Classes"", LoopVarName = ""_class_"")]]
            public partial class _class_ClassName
            {          }
            #endregion
        }
#endregion
#endregion
");
            var regions = tree.GetAllRegionDirectives(); //This is an array that correctly includes two SyntaxTrivia nodes
            var newAnnotation1 = new SyntaxAnnotation("test", "Data");
            var region1 = regions.First().WithAdditionalAnnotations(newAnnotation1);
            var newAnnotation2 = new SyntaxAnnotation("test");
            var region2 = regions.Last().WithAdditionalAnnotations(newAnnotation2);
            var newAnnotation3 = new SyntaxAnnotation("test2");
            var region3 = regions.Last().WithAdditionalAnnotations(newAnnotation3);
            var result1 = region1.GetAnnotations("test").First().Kind;
            var result2 = region1.GetAnnotations("test").First().Data;
            var result3 = region2.GetAnnotations("test").First().Kind;
            var result4 = region3.GetAnnotations("test2"). Last().Kind;
            Assert.AreEqual("test", result1);
            Assert.AreEqual("Data", result2);
            Assert.AreEqual("test", result3);
            Assert.AreEqual("test2", result4);
        }

        [TestMethod]
        public void Should_set_and_see_annotation2()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region Whatever
#region Whenever
       namespace _namespaceName
        {
            #region Fred
            public partial class _class_ClassName
            {          }
            #endregion
        }
#endregion
#endregion
");
            var rewriter = new TestWalker();
            CompilationUnitSyntax newCompilationUnit = (CompilationUnitSyntax)rewriter.Visit(tree.GetCompilationUnitRoot());
            var newTree = newCompilationUnit.SyntaxTree;
            var regions = newTree.GetAllRegionDirectives(); 
            var region1 = regions.First();
            var region2 = regions.Last();
            var result1 = region1.GetAnnotations("test").First().Kind;
            var result2 = region1.GetAnnotations("test").First().Data;
            var result3 = region2.GetAnnotations("test").First().Kind;
            Assert.AreEqual("test", result1);
            Assert.AreEqual("Data", result2);
            Assert.AreEqual("test", result3);
        }

        private class TestWalker : CSharpSyntaxRewriter
        {


            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            {
                var newAnnotation1 = new SyntaxAnnotation("test", "Data");
                trivia = trivia.WithAdditionalAnnotations(newAnnotation1);
                return base.VisitTrivia(trivia);
            }

            public override SyntaxNode VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
            {
                return base.VisitRegionDirectiveTrivia(node);
            }


        }

        [TestMethod]
        public void Can_find_end_of_simple_region()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region Whatever
 namespace kad_Template
    {
        namespace _namespaceName
       
    }
#endregion
");
            var regions = tree.GetAllRegionDirectives();
            var region = regions.First();
            var structure = (RegionDirectiveTriviaSyntax)region.GetStructure();
            var endRegion = structure.GetMatchingEndRegionDirective( );
            var location = endRegion.GetLocation();
            Assert.AreEqual(7, location.GetLineSpan(false).StartLinePosition.Line);
        }


        [TestMethod]
        public void Can_find_end_of_nested_region_inner()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region Whatever
 namespace kad_Template
 #region Whenever
       namespace _namespaceName
        {
            #region Fred
            public partial class _class_ClassName
            {          }
            #endregion
        }
#endregion
#endregion
");
            var regions = tree.GetAllRegionDirectives();
            var region = regions.Last();
            var structure = (RegionDirectiveTriviaSyntax)region.GetStructure();
            var endRegion = structure.GetMatchingEndRegionDirective();
            var location = endRegion.GetLocation();
            Assert.AreEqual(9, location.GetLineSpan(false).StartLinePosition.Line);
        }

        [TestMethod]
        public void Can_find_end_of_nested_region_outer()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region Whatever
 namespace kad_Template
 #region Whenever
       namespace _namespaceName
        {
            #region Fred
            public partial class _class_ClassName
            {          }
            #endregion
        }
#endregion
#endregion
");
            var regions = tree.GetAllRegionDirectives();
            var region = regions.First();
            var structure = (RegionDirectiveTriviaSyntax)region.GetStructure();
            var endRegion = structure.GetMatchingEndRegionDirective();
            var location = endRegion.GetLocation();
            Assert.AreEqual(12, location.GetLineSpan(false).StartLinePosition.Line);
        }

        [TestMethod]
        public void Can_find_end_of_nested_region_middle()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region Whatever
 namespace kad_Template
 #region Whenever
       namespace _namespaceName
        {
            #region Fred
            public partial class _class_ClassName
            {          }
            #endregion
        }
#endregion
#endregion
");
            var regions = tree.GetAllRegionDirectives();
            var region = regions.Skip(1).First();
            var structure = (RegionDirectiveTriviaSyntax)region.GetStructure();
            var endRegion = structure.GetMatchingEndRegionDirective();
            var location = endRegion.GetLocation();
            Assert.AreEqual(11, location.GetLineSpan(false).StartLinePosition.Line);
        }

        [TestMethod]
        public void Should_be_intelligent_when_end_region_missing()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region Whatever
 namespace kad_Template
 #region Whenever
       namespace _namespaceName
        {
            #region Fred
            public partial class _class_ClassName
            {          }
            #endregion
        }
#endregion
");
            var regions = tree.GetAllRegionDirectives();
            var region = regions.First();
            var structure = (RegionDirectiveTriviaSyntax)region.GetStructure();
            var endRegion = structure.GetMatchingEndRegionDirective();
            Assert.IsNull(endRegion);
        }

        [TestMethod]
        public void Should_build_SyntaxList_from_region()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region Whatever
 namespace kad_Template
 #region Whenever
       namespace _namespaceName
        {
            #region Fred
            public partial class _class_ClassName
            {          }
            #endregion
        }
#endregion
     namespace _namespaceName2
        {
            #region Fred
            public partial class _class_ClassName
            {          }
            #endregion
        }
#endregion
");
            var regions = tree.GetAllRegionDirectives();
            var region = regions.Skip(1).First();
            var contents = region.GetRegionContents();
            Assert.AreEqual(1, contents.Count());
            Assert.AreEqual("_namespaceName", contents.First().GetName());
        }

        [TestMethod]
        public void Should_remove_trivia_from_node()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
#region Whatever
 namespace kad_Template
 #region Whenever
       namespace _namespaceName
        {
        }
#endregion

#endregion
");
            var regions = tree.GetAllRegionDirectives();
            var region = regions.Skip(1).First();
            var node = region.Token.Parent;
            var newNode = node.RemoveTrivia(region);
            var oldNodeRegions = node.GetLeadingTrivia()
                    .Where(x => x.CSharpKind() == SyntaxKind.RegionDirectiveTrivia);
            var newNodeRegions = newNode.GetLeadingTrivia()
                    .Where(x => x.CSharpKind() == SyntaxKind.RegionDirectiveTrivia);
            Assert.AreEqual(1, oldNodeRegions.Count());
            Assert.AreEqual(0, newNodeRegions.Count());
        }
#endregion  

    }



}
