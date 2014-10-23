using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomBlockStatementFactory
            : RDomBaseItemFactory<RDomBlockStatement, BlockSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomBlockStatementFactory(RDomCorporation corporation)
              : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.Add(LanguageElement.CodeBlockStart, SyntaxKind.OpenBraceToken);
                    _whitespaceLookup.Add(LanguageElement.CodeBlockEnd, SyntaxKind.CloseBraceToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as BlockSyntax;
            var newItem = new RDomBlockStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statements = Corporation.Create(statementSyntax, newItem, model).OfType<IStatementCommentWhite>();
                newItem.Statements.AddOrMoveRange(statements);
            }

            return newItem;
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IBlockStatement;
            var block = BuildSyntaxWorker.GetStatementBlock(itemAsT.Statements);

            var node = SyntaxFactory.Block(SyntaxFactory.List(block.Statements));

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            return node.PrepareForBuildSyntaxOutput(item);

        }
    }

}
