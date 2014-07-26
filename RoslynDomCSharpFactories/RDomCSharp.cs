using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomCSharp
    {
        private static RDomCSharp _factory = new RDomCSharp();

        private RDomCSharp() { }

        public static RDomCSharp Factory
        { get { return _factory; } }

        public IRoot GetRootFromFile(string fileName)
        {
            var code = File.ReadAllText(fileName);
            return GetRootFromString(code);
        }

        public IRoot GetRootFromString(string code)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            return GetRootFromSyntaxTree(tree);
        }

        public IRoot GetRootFromDocument(Document document)
        {
            if (document == null) { throw new InvalidOperationException(); }
            SyntaxTree tree = document.GetSyntaxTreeAsync().Result;
            return GetRootFromSyntaxTree(tree);
        }

        public IRoot GetRootFromSyntaxTree(SyntaxTree tree)
        {
            CSharpFactory.Register();
            var compilation = CSharpCompilation.Create("MyCompilation",
                                           options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                                           syntaxTrees: new[] { tree },
                                           references: new[] { new MetadataFileReference(typeof(object).Assembly.Location) });
            var model = compilation.GetSemanticModel(tree);
            var rootFactoryHelper = RDomFactoryHelper.GetHelperForRoot();
            var root = rootFactoryHelper.MakeItems(tree.GetCompilationUnitRoot(), null, model).FirstOrDefault();
            return root;
        }


        public IEnumerable<SyntaxNode> BuildSyntaxGroup(IDom item)
        {
            IEnumerable<SyntaxNode> syntaxNodes;
            if (TryBuildSyntax<IRoot>(item, RDomFactoryHelper.GetHelperForRoot(), out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IStemMemberCommentWhite>(item, RDomFactoryHelper.GetHelperForStemMember(), out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<ITypeMemberCommentWhite>(item, RDomFactoryHelper.GetHelperForTypeMember(), out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IStatementCommentWhite>(item, RDomFactoryHelper.GetHelperForStatement(), out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IExpression>(item, RDomFactoryHelper.GetHelperForExpression(), out syntaxNodes)) { return syntaxNodes; }
            if (TryBuildSyntax<IMisc>(item, RDomFactoryHelper.GetHelperForMisc(), out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<IRoot>(item, out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<IStemMemberCommentWhite>(item, out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<ITypeMemberCommentWhite>(item, out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<IStatementCommentWhite>(item, out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<IExpression>(item, out syntaxNodes)) { return syntaxNodes; }
            //if (TryBuildSyntax<IMisc>(item, out syntaxNodes)) { return syntaxNodes; }
            return new List<SyntaxNode>();
        }

        public SyntaxNode BuildSyntax(IDom item)
        {
            return BuildSyntaxGroup(item).Single();
            //return RoslynUtilities.Format(BuildSyntaxGroup(item).Single());
        }

        private bool TryBuildSyntax<TKind>(IDom item, RDomFactoryHelper helper, out IEnumerable<SyntaxNode> syntaxNode)
             where TKind : class, IDom
        {
            syntaxNode = null;
            var itemAsKind = item as TKind;
            if (itemAsKind == null) { return false; }
            //var helper = RDomFactoryHelper.GetHelper<TKind>();
            syntaxNode = helper.BuildSyntaxGroup(item);
            return true;
        }
        //private bool TryBuildSyntax<TKind>(IDom item, out IEnumerable<SyntaxNode> syntaxNode)
        //    where TKind : class, IDom
        //{
        //    syntaxNode = null;
        //    var itemAsKind = item as TKind;
        //    if (itemAsKind == null) { return false; }
        //    var helper = RDomFactoryHelper.GetHelper<TKind>();
        //    syntaxNode = helper.BuildSyntaxGroup(item);
        //    return true;
        //}

    }

}
