using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Practices.Unity;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomFactory
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
            var rootFactoryHelper = RDomFactoryHelper.RootFactoryHelper;
            var root = rootFactoryHelper.MakeItem(tree.GetCompilationUnitRoot()).FirstOrDefault();
            return root;
        }

        public static IEnumerable<SyntaxNode> BuildSyntax(IDom item)
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

        private static bool TryBuildSyntax<TKind>(IDom item, out IEnumerable<SyntaxNode> syntaxNode)
            where TKind : class
        {
            syntaxNode = null;
            var itemAsKind = item as TKind;
            if (itemAsKind == null){ return false; }
            var helper = RDomFactoryHelper.GetHelper<TKind>();
            syntaxNode = helper.BuildSyntax(item);
            return true;
        }

    }

}
