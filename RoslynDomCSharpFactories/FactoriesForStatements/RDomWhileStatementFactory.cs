using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomWhileStatementFactory
         : RDomBaseLoopStatementFactory<RDomWhileStatement, WhileStatementSyntax>
    {
        //private static WhitespaceKindLookup _whitespaceLookup;

        public RDomWhileStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        //private WhitespaceKindLookup WhitespaceLookup
        //{
        //    get
        //    {
        //        if (_whitespaceLookup == null)
        //        {
        //            _whitespaceLookup = new WhitespaceKindLookup();
        //            _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
        //        }
        //        return _whitespaceLookup;
        //    }
        //}

        //protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        //{
        //    var syntax = syntaxNode as WhileStatementSyntax;
        //    var newItem = new RDomWhileStatement(syntaxNode, parent, model);
        //    CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
        //    CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

        //    newItem.TestAtEnd = false; // restating the obvious
        //    return LoopFactoryHelper.CreateItemFrom<IWhileStatement>(newItem, 
        //            syntax.Condition, syntax.Statement, parent, model, 
        //            Corporation, CreateFromWorker, WhitespaceLookup );
        //}

        //public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        //{
        //    var itemAsT = item as IWhileStatement;
        //    return LoopFactoryHelper.BuildSyntax<IWhileStatement>(itemAsT, 
        //        (c, s) => SyntaxFactory.WhileStatement(c, s), WhitespaceLookup);
        //}

        protected override ExpressionSyntax GetConditionFromSyntax(WhileStatementSyntax syntax)
        {
            return syntax.Condition;
        }

        protected override StatementSyntax GetStatementFromSyntax(WhileStatementSyntax syntax)
        {
            return syntax.Statement;
        }

        protected override RDomWhileStatement MakeNewItem(WhileStatementSyntax syntax, IDom parent, SemanticModel model)
        {
            return new RDomWhileStatement(syntax, parent, model);
        }

        protected override WhileStatementSyntax MakeSyntax(
                RDomWhileStatement itemAsT, ExpressionSyntax condition, StatementSyntax statementBlock)
        {
            return SyntaxFactory.WhileStatement( condition,statementBlock);
        }
    }
}
