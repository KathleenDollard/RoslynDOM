using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomDeclarationStatementFactory
        : RDomStatementFactory<RDomDeclarationStatement, VariableDeclaratorSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomDeclarationStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return syntaxNode is LocalDeclarationStatementSyntax;
        }

        protected override IEnumerable<IStatementCommentWhite> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var list = new List<IStatementCommentWhite>();
            var rawDeclaration = syntaxNode as LocalDeclarationStatementSyntax;

            // VariableDeclarationFactory does most of the work, and at present returns a single
            // DeclarationStatement, and possibly a comment
            var newItems = Corporation.CreateFrom<IMisc>(rawDeclaration.Declaration, parent, model, true);
            foreach (var newItem in newItems.OfType<IDeclarationStatement>())
            {
                CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);
                newItem.IsConst = rawDeclaration.IsConst;
            }
            list.AddRange(newItems.OfType<IStatementCommentWhite>());
            return list;
        }


        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IDeclarationStatement;
            var nameSyntax = SyntaxFactory.Identifier(itemAsT.Name);
            TypeSyntax typeSyntax;
            if (itemAsT.IsImplicitlyTyped)
            {
                typeSyntax = SyntaxFactory.ParseTypeName("var");
                typeSyntax = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast (typeSyntax, itemAsT.Type.Whitespace2Set.First());
            }
            else
            { typeSyntax = (TypeSyntax)RDomCSharp.Factory.BuildSyntaxGroup(itemAsT.Type).First(); }

            var variable = item as IVariableDeclaration;
            // This is a weakness in the current factory lookup - we can't just ask for a random factory
            // so to call the normal build syntax through the factory causes infinite recursion. 
            // TODO: Add the ability to request a random factory from the container (via the CSharp uber factory
            var tempFactory = new RDomVariableDeclarationFactory(Corporation);
            var nodeDeclarators = tempFactory.BuildSyntax(item);
            var nodeDeclarator = (VariableDeclaratorSyntax)nodeDeclarators.First();

            var nodeDeclaratorInList = SyntaxFactory.SeparatedList(
                                SyntaxFactory.List<VariableDeclaratorSyntax>(
                                    new VariableDeclaratorSyntax[]
                                            { nodeDeclarator }));
            var nodeDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, nodeDeclaratorInList);
            nodeDeclaration = BuildSyntaxHelpers.AttachWhitespace(nodeDeclaration, itemAsT.Whitespace2Set, WhitespaceLookup);
            var node = SyntaxFactory.LocalDeclarationStatement(nodeDeclaration);

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            return node.PrepareForBuildSyntaxOutput(item);
        }


    }
}
