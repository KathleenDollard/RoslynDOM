using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomCSharp
    {
        [ExcludeFromCodeCoverage]
        // until move to C# 6 - I want to support name of as soon as possible
        protected static string nameof<T>(T value) { return ""; }

        private static RDomCSharp _factory = new RDomCSharp();
        private RDomCorporation _helper = new RDomCorporation();

        private RDomCSharp() { }

        public static RDomCSharp Factory
        { get { return _factory; } }

        public IRoot GetRootFromFile(string fileName)
        {
            var code = File.ReadAllText(fileName);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            // TODO: Consider whether to expand the filename to full path
            return GetRootFromStringInternal(tree, fileName);
        }

        public IRoot GetRootFromString(string code)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            return GetRootFromStringInternal(tree, null);
        }

        public IRoot GetRootFromDocument(Document document)
        {
            Guardian.Assert.IsNotNull(document, nameof(document));
            SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
            return GetRootFromStringInternal(tree, document.FilePath);
        }

        public IRoot GetRootFromSyntaxTree(SyntaxTree tree)
        {
            return GetRootFromStringInternal(tree, tree.FilePath);
        }

        private IRoot GetRootFromStringInternal(SyntaxTree tree, string filePath)
        {
            //CSharpFactory.Register();
            var compilation = CSharpCompilation.Create("MyCompilation",
                                           options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                                           syntaxTrees: new[] { tree },
                                           references: new[] { new MetadataFileReference(typeof(object).Assembly.Location) });
            var model = compilation.GetSemanticModel(tree);
            var root = _helper.CreateFrom<IRoot>(tree.GetCompilationUnitRoot(), null, model).FirstOrDefault();
            root.FilePath = filePath;
            return root;
        }

        public IEnumerable<SyntaxNode> BuildSyntaxGroup(IDom item)
        {
            IEnumerable<SyntaxNode> syntaxNodes;
            if (TryBuildSyntax<IRoot>(item, out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IStemMemberCommentWhite>(item, out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<ITypeMemberCommentWhite>(item, out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IStatementCommentWhite>(item, out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IExpression>(item, out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IMisc>(item, out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<IRoot>(item, RDomFactoryHelper.GetHelperForRoot(), out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<IStemMemberCommentWhite>(item, RDomFactoryHelper.GetHelperForStemMember(), out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<ITypeMemberCommentWhite>(item, RDomFactoryHelper.GetHelperForTypeMember(), out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<IStatementCommentWhite>(item, RDomFactoryHelper.GetHelperForStatement(), out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<IExpression>(item, RDomFactoryHelper.GetHelperForExpression(), out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<IMisc>(item, RDomFactoryHelper.GetHelperForMisc(), out syntaxNodes)) { return syntaxNodes; }
            return new List<SyntaxNode>();
        }

        public SyntaxNode BuildSyntax(IDom item)
        {
            var syntaxGroup = BuildSyntaxGroup(item);
            if (syntaxGroup == null || !syntaxGroup.Any()) return null;
            return syntaxGroup.Single();
        }
        public SyntaxNode BuildFormattedSyntax(IDom item)
        {
            var syntax = BuildSyntax(item);
            return BuildSyntaxHelpers.Format(syntax, item);
        }

        private bool TryBuildSyntax<TKind>(IDom item, out IEnumerable<SyntaxNode> syntaxNode)
             where TKind : class, IDom
        {
            syntaxNode = null;
            var itemAsKind = item as TKind;
            if (itemAsKind == null) { return false; }
            //var corporation = RDomFactoryHelper.GetHelper<TKind>();
            syntaxNode = _helper.BuildSyntaxGroup(item);
            return true;
        }

        private IEnumerable<Tuple<Type, int>> expectations = new List<Tuple<Type, int>>()
        {
                    Tuple.Create(typeof(IMisc),2),
                    Tuple.Create(typeof(IExpression),1),
                    Tuple.Create(typeof(IStatementCommentWhite),6),
                    Tuple.Create(typeof(ITypeMemberCommentWhite),6),
                    Tuple.Create(typeof(IStemMemberCommentWhite),6),
                    Tuple.Create(typeof(IRoot),1),
                    Tuple.Create(typeof(IPublicAnnotation),1),
                    Tuple.Create(typeof(IAttribute),1),
                    Tuple.Create(typeof(ICommentWhite),1)
        };

        public bool ContainerCheck()
        {
            if (!_helper.HasExpectedItems())
            {
                Guardian.Assert.BadContainer();
                return false;
            }

            foreach (var tuple in expectations)
            {
                if (_helper.CountFactorySet(tuple.Item1) < tuple.Item2)
                {
                    Guardian.Assert.BadContainer();
                    return false;
                }
            }
            return true;
        }

        public string Report(IDom item)
        {
            return Report(item, false);

        }

        public virtual string Report(IDom item, bool includeTrivia)
        {
            var sb = new StringBuilder();
            var spaces = 2;
            var indentToAdd = new string(' ', spaces);
            var indent = "";
            var reversedAncestors = item.Ancestors.Reverse();
            foreach (var ancestor in reversedAncestors)
            {
                sb.AppendLine(indent + ancestor.ToString());
                indent += indentToAdd;
            }
            sb.AppendLine(indent + this.ToString());
            AppendChildHierarchy(item, sb, indent + indentToAdd, indentToAdd, includeTrivia);
            return sb.ToString();
        }

        private static void AppendChildHierarchy(IDom item, StringBuilder sb,
            string indent, string indentToAdd, bool includeTrivia)
        {
            foreach (var child in item.Children)
            {
                sb.AppendLine(indent + child.ToString());
                if (includeTrivia)
                { ReportTrivia(child, sb, indent); }
                AppendChildHierarchy(child, sb, indent + indentToAdd, indentToAdd, includeTrivia);
            }
        }

        private static void ReportTrivia(IDom child, StringBuilder sb, string indent)
        {
            var item = child as IRoslynDom;
            if (item == null) return;
            // Eventually flesh this out to all trivia types
            foreach (var ws in item.TokenWhitespaceSet.TokenWhitespaceList )
            {
                sb.AppendLine(indent + "- WS - " + ws.Token.ToString() + " - Leading='" + ws.LeadingWhitespace.Normalize() + "'; Trailing='" + ws.TrailingWhitespace.Normalize() + "'");
            }
        }
    }

}
