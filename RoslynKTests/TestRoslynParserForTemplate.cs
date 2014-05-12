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

//SpecialUnit 
//MetadataContext
//GetMostRecentProperty
//GetSpecialAttributes
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
    public class TestRoslynParserForTemplate
    {
        private string testDirective = "IncludesInterface";

        private string treeText
        { get { return @"
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using KadGen.KadEventSource;
using KadGen.Common.Roslyn;

namespace _namespaceName
{
    [_kad_NoOp()]
    [_kad_ForEach(LoopOver=""Meta"", LoopVarName=""_class_"")]
    public partial class _class_className
#if !IncludesInterface
        : EventSource
#endif
        {
            private Ikad_TemplateSupport _templateSupport;

            // Private constructor blocks direct instantiation of class
            private Meta_className() { }

            // Readonly access to cached, lazily created singleton instance
            private static readonly Lazy<_class_className> _lazyLog = new Lazy<_class_className>();
            public static _class_className Log
            {
                get { return _lazyLog.Value; }
            }

#if IncludesInterface
        // Readonly access to  private cached, lazily created singleton inner class instance
        private static readonly Lazy<_tmp_eventSourceClassName> _lazyInnerlog = new Lazy<_tmp_eventSourceClassName>();
        private static _tmp_eventSourceClassName _innerLog
        {
            get { return _lazyInnerlog.Value; }
        }

        [_kad_ForEach(LoopOver =""_class_.Events"", LoopVarName=""_event_"")]
        void _delegateEach(IEnumerable<KadEvent> _meta_Events,
            _tmp_eventSourceClassName _innerLog)
        { }

        private sealed partial class _tmp_eventSourceClassName : EventSource
        {
#endif

            #region Your trace event methods

            //TODO: Work out attributes
            [_kad_ForEach(LoopOver = ""_class_.Events"", LoopVarName = ""_event2_"")]
            public void _event_Name(ParameterDeclaration Parameters)
            {
                if (IsEnabled())
                    WriteEvent(_event_.EventId, _event_.Parameters);
                _templateSupport.MethodCall<int>(WriteEvent, _event_.EventId, _event_.Parameters);
            }
            #endregion
        }
#if IncludesInterface
    }
#endif
    }
"; } }
        private string treeTextTemp
        { get { return @"
/*
[_kad_ForEach(LoopOver=""Meta""
        , LoopVarName=""_class_"", 
OutputFileName=""_class_ClassName"")]
*/
namespace kad_Template
{
    using System;
    namespace _class_NamespaceName
    {
        public partial class _class_NamespaceName
        {

        }
        public partial class _class_EventSourceName
        {
           #region Your trace event methods

            [_kad_ForEach(LoopOver = ""_class_.Events"", LoopVarName = ""_event_"")]
            public void _event_Name(ParameterDeclaration Parameters)
            {
                if (IsEnabled())
                    WriteEvent(_class_EventSourceName, _event_EventId, _event_Parameters);
// _class_EventSourceName 
                _templateSupport.MethodCall<int>(WriteEvent, _event_.EventId, _event_Parameters);
            }
            #endregion
        }
    }
}
";

               //                   private string treeTextTemp
               //        { get { return @"
               //[_kad_metadataExtension]
               //public static class kad_MetadataExtension<KadEventSource>
               //{
               //    public static void Initialize(KadEventSource initial)
               //    {
               //        var tempName = this.Name.SubstringAfterLast(""-"");
               //        if (tempName != initial.ClassName)
               //                { initial.ClassName = tempName; }
               //    }
               //}

               //[_kad_metadataExtension]
               //public static class kad_MetadataExtension<KadEvent>
               //{
               //    public static void Initialize(KadEvent initial)
               //    {  }
               //}

               //[_kad_NoOp()]
               //namespace kad_Template
               //{
               //    [_kad_ForEach(LoopOver=""Meta"", LoopVarName=""_class_"", FileName=""className"")]
               //    using System;
               //    using System.Collections.Generic;
               //    using System.Diagnostics.Tracing;
               //    using KadGen.KadEventSource;
               //    using KadGen.Common.Roslyn;

               //    namespace _namespaceName
               //    {
               //        public partial class _class_className
               //        {
               //                private Ikad_TemplateSupport _templateSupport;

               //        }
               //    }
               //}
               //"; 
            } }


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
        public void Can_create_new_metadata_context()
        {
            var metadata = new RoslynTemplateGenerator<KadEventSource>.MetadataContext();
            Assert.IsNotNull(metadata);
        }

        [TestMethod]
        public void Can_add_to_and_retrieve_from_metadata_context()
        {
            var metadata = new RoslynTemplateGenerator<KadEventSource>.MetadataContext();
            metadata.AddLookupValue("foo", "bar");
            metadata.AddLookupValue("universe", 42);
            metadata.AddLookupValue("arrayOfStrings", new string[] { "1", "2", "3" });
            metadata.AddLookupValue("nestStruct", new testNestingStruct()
                            { First = "one", Second = 2 });
            metadata.AddLookupValue("nestClass", new testNestingClass()
                            { Third = "three", Fourth = 4 });
            object result1;
            object result2;
            IEnumerable<object> result3;
            object result4;
            object result5;
            Assert.IsTrue(metadata.TryGetValue("foo", out result1));
            Assert.IsTrue(metadata.TryGetValue("universe", out result2));
            Assert.IsTrue(metadata.TryGetValues("arrayOfStrings", out result3));
            Assert.IsTrue(metadata.TryGetValue("nestStruct", out result4));
            Assert.IsTrue(metadata.TryGetValue("nestClass", out result5));
            Assert.AreEqual("bar", result1);
            Assert.AreEqual(42, result2);
            Assert.AreEqual("1", result3.First());
            Assert.AreEqual("3", result3.Last());
            Assert.AreEqual("one",((testNestingStruct) result4).First);
            Assert.AreEqual(2, ((testNestingStruct)result4).Second);
            Assert.AreEqual("three", ((testNestingClass)result5).Third);
            Assert.AreEqual(4, ((testNestingClass)result5).Fourth);
        }
        public struct testNestingStruct
        {
            public string First { get; set; }
            public int Second { get; set; }
        }

        public class testNestingClass
        {
            public string Third { get; set; }
            public int Fourth { get; set; }
        }

        [TestMethod]
        public void Can_clone_metadata_context()
        {
            var metadata = new RoslynTemplateGenerator<KadEventSource>.MetadataContext();
            metadata.AddLookupValue("foo", "bar");
            metadata.AddLookupValue("universe", 42);
            var metadata2 = metadata.Clone();
            object result1;
            object result2;
            Assert.IsTrue(metadata2.TryGetValue("foo", out result1));
            Assert.IsTrue(metadata2.TryGetValue("universe", out result2));
            Assert.AreEqual("bar", result1);
            Assert.AreEqual(42, result2);
        }



        [TestMethod]
        public void  Can_loop_with_kad_for_each()
        {
            var parser = new KadGen.Common.Roslyn.ParserForTemplate();
            var templateSets = parser.Parse(treeTextTemp);
            var generator1 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithInterface);
            var newCompilation = generator1.Evaluate();
            var namespaces = newCompilation.GetNamespaces();
            Assert.AreEqual(1,namespaces.Count());
            var newNamespace = namespaces.First();
            var classes = newNamespace.GetClasses();
            Assert.AreEqual(2, classes.Count());
            Assert.Inconclusive();
        }

        #region [_kad_FileForEach(LoopOver="Meta", LoopVarName="_class_")] 
        #region This is a region
        #region B
        #endregion
        #endregion
       #endregion
        [TestMethod]
        public void Can_retrieve_from_special_unit()
        {
            var parser = new KadGen.Common.Roslyn.ParserForTemplate();
            var templateSets = parser.Parse(treeText);
            var generator1 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithInterface);
            var root1 = generator1.InitialTemplateTree.GetCompilationUnitRoot();
            var nspace1 = root1.GetNamespaces().First();
            var class1 = nspace1.GetClasses().First();
            var class1SpecialUnits = generator1.GetSpecialUnits(class1);
            Assert.AreEqual(2, class1SpecialUnits.Count());
            Assert.IsNotNull(class1SpecialUnits.First());
            Assert.IsNotNull(class1SpecialUnits.First().Node);
            Assert.IsNotNull(class1SpecialUnits.First().Attribute);
            var method1 = class1.GetMethodByName("_event_Name");
            if (method1 == null)
            {
                var class2 = class1.GetClasses().First();
                method1 = class2.GetMethodByName("_event_Name");
            }
            var method1SpecialUnits = generator1.GetSpecialUnits(method1);
            Assert.AreEqual(1, method1SpecialUnits.Count());
            Assert.IsNotNull(method1SpecialUnits.First());
            Assert.IsNotNull(method1SpecialUnits.First().Node);
            Assert.IsNotNull(method1SpecialUnits.First().Attribute);
        }

        [TestMethod]
        public void GetMostRecentProperty_finds_most_recent_with_multiples()
        {
            Assert.Inconclusive();
        }

        //[TestMethod]
        //public void GetMostRecentProperty_returns_correct_when_found()
        //{
        //    Assert.Inconclusive();
        //}

        //[TestMethod]
        //public void GetMostRecentProperty_returns_null_when_not_found()
        //{
        //    Assert.Inconclusive();
        //}

        [TestMethod]
        public void GetSpecialAttributes_returns_empty_list_when_not_found()
        {
            var parser = new KadGen.Common.Roslyn.ParserForTemplate();
            var templateSets = parser.Parse(treeText);
            var generator1 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithInterface);
            var root1 = generator1.InitialTemplateTree.GetCompilationUnitRoot();
            var nspace1 = root1.ChildNodes().Where(x => x.CSharpKind() == SyntaxKind.NamespaceDeclaration).First();
            var attribs = nspace1.GetSpecialAttributes(generator1.SpecialAttributePredicates);
            Assert.AreEqual(0, attribs.Count());
        }

        [TestMethod]
        public void GetSpecialAttributes_returns_multiples_when_found()
        {
            var parser = new KadGen.Common.Roslyn.ParserForTemplate();
            var templateSets = parser.Parse(treeText);
            var generator1 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithInterface);
            var root1 = generator1.InitialTemplateTree.GetCompilationUnitRoot();
            var nspace1 = root1.ChildNodes().Where(x => x.CSharpKind() == SyntaxKind.NamespaceDeclaration).First();
            var class1 = nspace1.ChildNodes().Where(x => x.CSharpKind() == SyntaxKind.ClassDeclaration).First();
            var attribs = class1.GetSpecialAttributes(generator1.SpecialAttributePredicates);
            Assert.AreEqual(2, attribs.Count());
        }

        [TestMethod]
        public void GetSpecialAttributes_returns_single_when_found()
        {
            var parser = new KadGen.Common.Roslyn.ParserForTemplate();
            var templateSets = parser.Parse(treeText);
            var generator1 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithInterface);
            var root1 = generator1.InitialTemplateTree.GetCompilationUnitRoot();
            var nspace1 = root1.ChildNodes().Where(x => x.CSharpKind() == SyntaxKind.NamespaceDeclaration).First();
            var class1 = nspace1.ChildNodes().Where(x => x.CSharpKind() == SyntaxKind.ClassDeclaration).First();
            var methods = class1.ChildNodes().Where(x => x.CSharpKind() == SyntaxKind.MethodDeclaration);
            var method1 = methods.Last();
            var attribs = method1.GetSpecialAttributes(generator1.SpecialAttributePredicates);
            Assert.AreEqual(1, attribs.Count());
        }

        [TestMethod]
        public void Can_expand_template()
        {

            var parser = new KadGen.Common.Roslyn.ParserForTemplate();
            var templateSets = parser.Parse(treeText);
            //Assert.Are
        }

        [TestMethod]
        public void Can_find_special_kad_units()
        {

            var parser = new KadGen.Common.Roslyn.ParserForTemplate();
            var templateSets = parser.Parse(treeText);
            var generator1 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithInterface);
            var generator2 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithoutInterface);
            var specialUnits1 = generator1.GetSpecialUnits(true);
            var specialUnits2 = generator2.GetSpecialUnits(true);
            Assert.AreEqual(4, specialUnits1.Count());
            Assert.IsInstanceOfType(specialUnits1.First().Node, typeof(ClassDeclarationSyntax));
            Assert.IsInstanceOfType(specialUnits1.Skip(1).First().Node, typeof(ClassDeclarationSyntax));
            Assert.IsInstanceOfType(specialUnits1.Skip(2).First().Node, typeof(MethodDeclarationSyntax));
            Assert.IsInstanceOfType(specialUnits1.Last().Node, typeof(MethodDeclarationSyntax));
            Assert.AreEqual(3, specialUnits2.Count());
            Assert.IsInstanceOfType(specialUnits2.First().Node, typeof(ClassDeclarationSyntax));
            Assert.IsInstanceOfType(specialUnits2.Last().Node, typeof(MethodDeclarationSyntax));
        }

        [TestMethod]
        public void Can_pick_correct_template_set()
        {

            var parser = new KadGen.Common.Roslyn.ParserForTemplate();
            var templateSets = parser.Parse(treeText);
            var generator1 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithInterface);
            var generator2 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithoutInterface);
            var tree1 = generator1.InitialTemplateTree;
            var tree2 = generator2.InitialTemplateTree;
            Assert.AreEqual(1, tree1.GetClasses().Count());
            Assert.AreEqual(1, tree2.GetClasses().Count());
        }

        [TestMethod]
        public void Can_get_root_from_correct_template_set()
        {

            var parser = new KadGen.Common.Roslyn.ParserForTemplate();
            var templateSets = parser.Parse(treeText);
            var generator1 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithInterface);
            var generator2 = new RoslynTemplateGenerator<KadEventSource>(templateSets, eventSourceWithoutInterface);
            var tree1 = generator1.InitialTemplateTree;
            var tree2 = generator2.InitialTemplateTree;
            Assert.AreEqual(1, tree1.GetClasses().Count());
            Assert.AreEqual(1, tree2.GetClasses().Count());
        }

        [TestMethod]
        public void Can_find_kad_attributes()
        {
            var option1 = new CSharpParseOptions(LanguageVersion.CSharp5, DocumentationMode.Parse, SourceCodeKind.Regular, "IncludesInterface");
            SyntaxTree tree1 = CSharpSyntaxTree.ParseText(treeText, "", option1);
            var option2 = new CSharpParseOptions(LanguageVersion.CSharp5, DocumentationMode.Parse, SourceCodeKind.Regular);
            SyntaxTree tree2 = CSharpSyntaxTree.ParseText(treeText, "", option2);
            var unit1 = tree1.GetCompilationUnitRoot();
            var unit2 = tree2.GetCompilationUnitRoot();
            var attributes1 = unit1.GetDescendantAttributes(x => x.Name.ToString().StartsWith("_kad_"));
            var attributes2 = unit2.GetDescendantAttributes(x => x.Name.ToString().StartsWith("_kad_"));
            Assert.AreEqual(4, attributes1.Count());
            Assert.AreEqual(3, attributes2.Count());
            var parent1_1 = attributes1.First().Parent.Parent;
            var parent1_2 = attributes1.Skip(1).First().Parent.Parent;
            var parent1_3 = attributes1.Skip(2).First().Parent.Parent;
            var parent1_4 = attributes1.Last().Parent.Parent;
            Assert.IsInstanceOfType(parent1_1, typeof(ClassDeclarationSyntax));
            Assert.IsInstanceOfType(parent1_2, typeof(ClassDeclarationSyntax));
            Assert.IsInstanceOfType(parent1_3, typeof(MethodDeclarationSyntax));
            Assert.IsInstanceOfType(parent1_4, typeof(MethodDeclarationSyntax));
        }

        [TestMethod]
        public void Template_load_reflects_preprocessor_symbols()
        {
            var option1 = new CSharpParseOptions(LanguageVersion.CSharp5, DocumentationMode.Parse, SourceCodeKind.Regular, "IncludesInterface");
            SyntaxTree tree1 = CSharpSyntaxTree.ParseText(treeText, "", option1);
            var option2 = new CSharpParseOptions(LanguageVersion.CSharp5, DocumentationMode.Parse, SourceCodeKind.Regular);
            SyntaxTree tree2 = CSharpSyntaxTree.ParseText(treeText, "", option2);
            var unit1 = tree1.GetCompilationUnitRoot();
            var unit2 = tree2.GetCompilationUnitRoot();
            var nspace1 = (NamespaceDeclarationSyntax)unit1.Members.First();
            var nspace2 = (NamespaceDeclarationSyntax)unit2.Members.First();
            var class1 = (ClassDeclarationSyntax)nspace1.Members.First();
            var class2 = (ClassDeclarationSyntax)nspace2.Members.First();
            Assert.IsNull(class1.BaseList);
            Assert.AreEqual(1, class2.BaseList.Types.Count());
        }

        [TestMethod]
        public void Parse_returns_correct_template_sets()
        {

            var parser = new KadGen.Common.Roslyn.ParserForTemplate();
            var templateSets = parser.Parse(treeText);
            Assert.AreEqual(2, templateSets.Count());
            var set1 = templateSets.Where(x => string.Join(",", x.IfDirectiveSymbols) == "");
            var set2 = templateSets.Where(x => string.Join(",", x.IfDirectiveSymbols) == testDirective);
            Assert.AreEqual(1, set1.Count());
            Assert.AreEqual(1, set2.Count());
            var unit1 = set1.First().Tree.GetCompilationUnitRoot();
            var unit2 = set2.First().Tree.GetCompilationUnitRoot();
            Assert.IsNotNull(set1.First().Tree);
            Assert.IsNotNull(set2.First().Tree);
            Assert.AreEqual(1, unit1.DescendantNodes().Where(x => x.CSharpKind() == SyntaxKind.ClassDeclaration).Count());
            Assert.AreEqual(2, unit2.DescendantNodes().Where(x => x.CSharpKind() == SyntaxKind.ClassDeclaration).Count());
        }

        [TestMethod]
        public void Can_find_if_directive_symbols()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(treeText);
            var symbols = tree.GetIfDirectiveSymbols();
            Assert.AreEqual(testDirective, symbols.Single());
        }
    }
}
