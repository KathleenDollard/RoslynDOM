using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomCheckedStatementFactory
                : RDomBaseItemFactory<RDomCheckedStatement, CheckedStatementSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomCheckedStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.Add(LanguageElement.Checked, SyntaxKind.CheckedKeyword);
                    _whitespaceLookup.Add(LanguageElement.Unchecked, SyntaxKind.UncheckedKeyword );
                    _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
                    _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as CheckedStatementSyntax;
            var newItem = new RDomCheckedStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Block, newItem, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, syntax.Block, LanguagePart.Current, WhitespaceLookup);

            newItem.Unchecked = (syntax.CSharpKind() == SyntaxKind.UncheckedStatement);

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as ICheckedStatement;
            var statement = RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup) as BlockSyntax;
            var kind = itemAsT.Unchecked ? SyntaxKind.UncheckedStatement : SyntaxKind.CheckedStatement;

            var node = SyntaxFactory.CheckedStatement(kind, statement);


            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            return node.PrepareForBuildSyntaxOutput(item);
        }
    }
}
