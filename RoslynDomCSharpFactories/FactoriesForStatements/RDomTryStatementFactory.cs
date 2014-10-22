using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomTryStatementFactory
         : RDomStatementFactory<RDomTryStatement, TryStatementSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomTryStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.Add(LanguageElement.TryKeyword, SyntaxKind.TryKeyword);
                    _whitespaceLookup.Add(LanguageElement.CatchKeyword, SyntaxKind.CatchKeyword);
                    _whitespaceLookup.Add(LanguageElement.FinallyKeyword, SyntaxKind.FinallyKeyword);
                    _whitespaceLookup.Add(LanguageElement.ConditionalStartDelimiter, SyntaxKind.OpenParenToken);
                    _whitespaceLookup.Add(LanguageElement.ConditionalEndDelimiter, SyntaxKind.CloseParenToken);
                    _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
                    _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as TryStatementSyntax;
            var newItem = new RDomTryStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.InitializeStatements(newItem, syntax.Block, newItem, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, syntax.Block, LanguagePart.Current, WhitespaceLookup);

            var catchSyntaxList = syntax.ChildNodes()
                                    .Where(x => x.CSharpKind() == SyntaxKind.CatchClause)
                                    .OfType<CatchClauseSyntax>();
            foreach (var ctch in catchSyntaxList)
            {
                var newCatch = new RDomCatchStatement(ctch, newItem, model);
                CreateFromWorker.StandardInitialize(newCatch, ctch, newItem, model);
                CreateFromWorker.InitializeStatements(newCatch, ctch.Block, newCatch, model);
                CreateFromWorker.StoreWhitespace(newCatch, ctch, LanguagePart.Current, WhitespaceLookup);
                CreateFromWorker.StoreWhitespace(newCatch, ctch.Block, LanguagePart.Current, WhitespaceLookup);
                if (ctch.Declaration != null)
                {
                    var type = Corporation
                                  .CreateFrom<IMisc>(ctch.Declaration.Type, newCatch, model)
                                  .FirstOrDefault()
                                  as IReferencedType;
                    newCatch.ExceptionType = type;
                    CreateFromWorker.StoreWhitespace(newCatch, ctch.Declaration, LanguagePart.Current, WhitespaceLookup);
                    if (!string.IsNullOrWhiteSpace(ctch.Declaration.Identifier.ToString()))
                    {
                        newCatch.Variable = Corporation.CreateFrom<IMisc>(ctch.Declaration, newCatch, model).FirstOrDefault() as IVariableDeclaration;
                        newCatch.Variable.Type = type;
                    }
                }
                if (ctch.Filter != null)
                {
                    newCatch.Condition = Corporation.CreateFrom<IExpression>(ctch.Filter.FilterExpression, newCatch, model).FirstOrDefault();
                    CreateFromWorker.StoreWhitespace(newCatch.Condition, ctch.Filter, LanguagePart.Current, WhitespaceLookup);
                }
                newItem.CatchesAll.AddOrMove(newCatch);
            }
            if (syntax.Finally != null)
            {
                var newFinally = new RDomFinallyStatement(syntax.Finally, newItem, model);
                CreateFromWorker.StandardInitialize(newFinally, syntax.Finally, parent, model);
                CreateFromWorker.InitializeStatements(newFinally, syntax.Finally.Block, newFinally, model);
                CreateFromWorker.StoreWhitespace(newFinally, syntax.Finally, LanguagePart.Current, WhitespaceLookup);
                CreateFromWorker.StoreWhitespace(newFinally, syntax.Finally.Block, LanguagePart.Current, WhitespaceLookup);
                newItem.Finally = newFinally;
            }
            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as ITryStatement;
            var node = SyntaxFactory.TryStatement();
            var catches = BuildCatchSyntaxList(itemAsT);
            var fnally = BuildFinallySyntax(itemAsT);
            var block = BuildSyntaxWorker.GetStatementBlock(itemAsT.Statements);
            block = BuildSyntaxHelpers.AttachWhitespace(block, itemAsT.Whitespace2Set, WhitespaceLookup);

            node = node.WithCatches(SyntaxFactory.List(catches))
                     .WithFinally(fnally)
                     .WithBlock(block);

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            return node.PrepareForBuildSyntaxOutput(item);
        }

        private FinallyClauseSyntax BuildFinallySyntax(ITryStatement itemAsT)
        {
            var fnally = itemAsT.Finally;
            // TODO: Empty statement would return empty brackets here?
            var block = BuildSyntaxWorker.GetStatementBlock(fnally.Statements);
            block = BuildSyntaxHelpers.AttachWhitespace(block, fnally.Whitespace2Set, WhitespaceLookup);
            var syntax = SyntaxFactory.FinallyClause(block);
            syntax = BuildSyntaxHelpers.AttachWhitespace(syntax, fnally.Whitespace2Set, WhitespaceLookup);
            return syntax;
        }

        private IEnumerable<CatchClauseSyntax> BuildCatchSyntaxList(ITryStatement itemAsT)
        {
            var ret = new List<CatchClauseSyntax>();
            foreach (var ctch in itemAsT.Catches)
            {
                var syntax = SyntaxFactory.CatchClause();
                if (ctch.ExceptionType != null)
                {
                    TypeSyntax typeSyntax = (TypeSyntax)(RDom.CSharp.GetSyntaxNode(ctch.ExceptionType));
                    var declaration = SyntaxFactory.CatchDeclaration(typeSyntax);
                    if (ctch.Variable != null)
                    { declaration = declaration.WithIdentifier(SyntaxFactory.Identifier(ctch.Variable.Name)); }
                    declaration = BuildSyntaxHelpers.AttachWhitespace(declaration, ctch.Whitespace2Set, WhitespaceLookup);
                    syntax = syntax.WithDeclaration(declaration);
                }
                // TODO: Add catch filter for 6.0
                // TODO: Empty statement would return empty brackets here?
                var block = BuildSyntaxWorker.GetStatementBlock(ctch.Statements);
                block = BuildSyntaxHelpers.AttachWhitespace(block, ctch.Whitespace2Set, WhitespaceLookup);
                syntax = syntax.WithBlock(block);
                syntax = BuildSyntaxHelpers.AttachWhitespace(syntax, ctch.Whitespace2Set, WhitespaceLookup);
                ret.Add(syntax);
            }

            return ret;
        }
    }
}
