using KadGen.Common.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TestRoslyn
{

    [TestClass]
    public class TestRoslynSyntaxElementAttribute
    {

        private IEnumerable<Func<AttributeSyntax, bool>> specialPredicates =
            new List<Func<AttributeSyntax, bool>>() { y => y.Name.ToString().StartsWith("_kad_") };
        // THere are more attribute tests in TestRoslynSyntaxElementClass

        [TestMethod]
        public void SpecialAttributeByName_should_find_name_in_namespace()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
/*
[_kad_ForEach(LoopOver=""Meta""
        , LoopVarName=""_class_"", 
OutputFileName=""_class_ClassName"")]
*/
namespace kad_Template{}
");
            var attrib = tree.GetNamespaces().First().GetSpecialAttributeByName(specialPredicates, "_kad_ForEach");
            Assert.IsNotNull(attrib);
        }

        [TestMethod]
        public void SpecialAttributeByName_should_return_null_when_missing_in_namespace()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
/*
[_kad_ForEach(LoopOver=""Meta""
        , LoopVarName=""_class_"", 
OutputFileName=""_class_ClassName"")]
*/
namespace kad_Template { }
");
            var attrib = tree.GetNamespaces().First().GetSpecialAttributeByName(specialPredicates, "_kad_ForEachX");
            Assert.IsNull(attrib);
}

        [TestMethod]
        public void SpecialAttributeByName_should_find_name_in_class()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
/*
[_kad_ForEach(LoopOver=""Meta""
        , LoopVarName=""_class_"", 
OutputFileName=""_class_ClassName"")]
*/
namespace kad_Template
    {
        using System;
               [_kad_metadataExtension]
               public static class kad_MetadataExtension<KadEventSource>
               {
               }
    }
");
            var attrib = tree.GetNamespaces().First().GetSpecialAttributeByName(specialPredicates, "_kad_ForEach");
            Assert.IsNotNull(attrib);
        }

        [TestMethod]
        public void SpecialAttributeByName_should_return_null_when_missing_in_class()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
/*
[_kad_ForEach(LoopOver=""Meta""
        , LoopVarName=""_class_"", 
OutputFileName=""_class_ClassName"")]
*/
namespace kad_Template
    {
        using System;
               [_kad_metadataExtension]
               public static class kad_MetadataExtension<KadEventSource>
               {
               }
    }
");
            var attrib = tree.GetNamespaces().First().GetClasses().First().GetSpecialAttributeByName(specialPredicates, "_kad_ForEachx");
            Assert.IsNull(attrib);
        }

    }
}