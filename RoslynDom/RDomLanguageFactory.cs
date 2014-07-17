using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomLanguageFactory
    {
        public static IRoot GetRootFromFile(string fileName)
        {
            var code = File.ReadAllText(fileName);
            return GetRootFromString(code);
        }

        public static IRoot GetRootFromString(string code)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            return GetRootFromSyntaxTree(tree);
        }

        public static IRoot GetRootFromDocument(Document document)
        {
            if (document == null) { throw new InvalidOperationException(); }
            SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
            return GetRootFromSyntaxTree(tree);
        }

        public static IRoot GetRootFromSyntaxTree(SyntaxTree tree)
        {
            //var root2 = RDomFactory2.MakeRoot(tree);
            CSharpFactory.Register();
            var compilation = CSharpCompilation.Create("MyCompilation",
                                           options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                                           syntaxTrees: new[] { tree },
                                           references: new[] { new MetadataFileReference(typeof(object).Assembly.Location )});
            var model = compilation.GetSemanticModel(tree);
            var rootFactoryHelper = RDomFactoryHelper.GetHelper<IRoot>();
            var root = rootFactoryHelper.MakeItem(tree.GetCompilationUnitRoot(), model).FirstOrDefault();
            return root;
        }

        public static IEnumerable<SyntaxNode> BuildSyntaxGroup(IDom item)
        {
            IEnumerable<SyntaxNode> syntaxNodes;
            if (TryBuildSyntax<IRoot>(item, out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IStemMember>(item, out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<ITypeMember>(item, out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IStatement>(item, out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IExpression>(item, out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IMisc>(item, out syntaxNodes)) { return syntaxNodes; }
            return null;
        }

        public static SyntaxNode BuildSyntax(IDom item)
        {
            return BuildSyntaxGroup(item).Single();
        }

        private static bool TryBuildSyntax<TKind>(IDom item, out IEnumerable<SyntaxNode> syntaxNode)
            where TKind : class, IDom
        {
            syntaxNode = null;
            var itemAsKind = item as TKind;
            if (itemAsKind == null) { return false; }
            var helper = RDomFactoryHelper.GetHelper<TKind>();
            syntaxNode = helper.BuildSyntaxGroup(item);
            return true;
        }

    }

}
