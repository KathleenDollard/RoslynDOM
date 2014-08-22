using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomDoStatementFactory
         : RDomBaseLoopStatementFactory<RDomDoStatement, DoStatementSyntax>
    {
        //private static WhitespaceKindLookup _whitespaceLookup;

        public RDomDoStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        //private WhitespaceKindLookup WhitespaceLookup
        //{
        //    get
        //    {
        //        if (_whitespaceLookup == null)
        //        {
        //            _whitespaceLookup = new WhitespaceKindLookup();
        //            _whitespaceLookup.Add(LanguageElement.DoKeyword, SyntaxKind.DoKeyword);
        //            _whitespaceLookup.Add(LanguageElement.WhileKeyword, SyntaxKind.WhileKeyword);
        //            _whitespaceLookup.Add(LanguageElement.ConditionalStartDelimiter, SyntaxKind.OpenParenToken);
        //            _whitespaceLookup.Add(LanguageElement.ConditionalEndDelimiter, SyntaxKind.CloseParenToken);
        //            _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
        //            _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter, SyntaxKind.CloseBraceToken);
        //            _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
        //        }
        //        return _whitespaceLookup;
        //    }
        //}

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var newItem = base.CreateItemFrom(syntaxNode, parent, model) as RDomDoStatement;
            newItem.TestAtEnd = true;
            return newItem;
            //var syntax = syntaxNode as DoStatementSyntax;
            //var newItem = new RDomDoStatement(syntaxNode, parent, model);
            //CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            //CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

            //newItem.TestAtEnd = true;
            //return LoopFactoryHelper.CreateItemFrom<IDoStatement>(newItem,
            //        syntax.Condition, syntax.Statement, parent, model,
            //        Corporation, CreateFromWorker, WhitespaceLookup);
        }

        protected override ExpressionSyntax GetConditionFromSyntax(DoStatementSyntax syntax)
        {
            return syntax.Condition;
        }

        protected override StatementSyntax GetStatementFromSyntax(DoStatementSyntax syntax)
        {
            return syntax.Statement;
        }

        protected override RDomDoStatement MakeNewItem(DoStatementSyntax syntax, IDom parent, SemanticModel model)
        {
            return new RDomDoStatement(syntax, parent, model);
        }

        protected override DoStatementSyntax MakeSyntax(
                RDomDoStatement itemAsT, ExpressionSyntax condition, StatementSyntax statementBlock)
        {
            return SyntaxFactory.DoStatement(statementBlock, condition);
        }

        //  public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        //{
        //    var itemAsT = item as IDoStatement ;
        //    return LoopFactoryHelper.BuildSyntax<IDoStatement>(itemAsT, (c, s) => SyntaxFactory.DoStatement(s, c), WhitespaceLookup);
        //}


    }
}
