using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{

    public class RDomIfStatementFactory
         : RDomStatementFactory<RDomIfStatement, IfStatementSyntax>
    {

        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomIfStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.Add(LanguageElement.IfKeyword, SyntaxKind.IfKeyword);
                    _whitespaceLookup.Add(LanguageElement.ElseKeyword, SyntaxKind.ElseKeyword);
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
            var newItem = CreateCollapsing(syntaxNode as IfStatementSyntax, parent, model);
            return newItem;
        }

        private RDomIfStatement CreateCollapsing(IfStatementSyntax syntax, IDom parent, SemanticModel model)
        {
            // You can't use descendants here becuase it is a very specific pattern 
            var newItem = new RDomIfStatement(syntax, parent, model);
            UpdateItem(newItem, syntax.Statement, syntax.Condition, syntax, parent, model);
            var currentSyntax = syntax;
            IElseBaseStatement lastItem = newItem;

            while (currentSyntax != null)
            {
                if (currentSyntax.Else == null) // We're done
                { break; }
                var elseAsIf = currentSyntax.Else.Statement as IfStatementSyntax;
                if (elseAsIf != null)  
                {
                    var newElse = new RDomElseIfStatement(elseAsIf, newItem, model);
                    UpdateItem(newElse, elseAsIf.Statement, elseAsIf.Condition, elseAsIf, newItem, model);
                    CreateFromWorker.StoreWhitespaceForToken(newElse, currentSyntax.Else.ElseKeyword, LanguagePart.Current, LanguageElement.ElseKeyword);
                    newItem.Elses.AddOrMove(newElse);
                    lastItem = newElse;
                    currentSyntax = elseAsIf;
                }
                else // At terminus, add else and we're done
                {
                    var newElse = new RDomElseStatement(currentSyntax.Else, newItem, model);
                    UpdateItem(newElse, currentSyntax.Else.Statement, null, currentSyntax.Else, newItem, model);
                    CreateFromWorker.StoreWhitespaceForToken(newElse, currentSyntax.Else.ElseKeyword, LanguagePart.Inner, LanguageElement.ElseKeyword);
                    newItem.Elses.AddOrMove(newElse);
                    break;
                }
            }
            return newItem;
        }


        private void UpdateItem<T>(T newItem, StatementSyntax statement, ExpressionSyntax condition,
                   SyntaxNode syntax, IDom parent, SemanticModel model)
           where T : class, IDom, IStatementBlock
        {
            CreateFromWorker.StandardInitialize(newItem, syntax, parent, model);
            CreateFromWorker.InitializeStatements(newItem, statement, newItem, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, statement, LanguagePart.Block, WhitespaceLookup);
            var itemAsHasCondition = newItem as IHasCondition;
            if (itemAsHasCondition != null)
            {
                itemAsHasCondition.Condition = Corporation.CreateFrom<IExpression>(condition, newItem, model).FirstOrDefault();
                CreateFromWorker.StoreWhitespace(itemAsHasCondition, condition, LanguagePart.Current, WhitespaceLookup);
            }
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IIfStatement;
            var elseSyntax = BuildElseSyntax(itemAsT);
            var node = SyntaxFactory.IfStatement(GetCondition(itemAsT), GetStatement(itemAsT));
            if (elseSyntax != null) { node = node.WithElse(elseSyntax); }

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            return node.PrepareForBuildSyntaxOutput(item);
        }

        private ElseClauseSyntax BuildElseSyntax(IIfStatement itemAsT)
        {
            // Because we reversed the list, inner is first, inner to outer required for this approach
            var elses = itemAsT.Elses.Reverse();
            var lastItem = itemAsT;
            ElseClauseSyntax elseClause = null;
            foreach (var nestedElse in elses)
            {
                var statement = GetStatement(nestedElse);
                var elseIf = nestedElse as IElseIfStatement;
                if (elseIf != null)
                {
                    // build if statement and put in else clause
                    statement = SyntaxFactory.IfStatement(GetCondition(elseIf), statement)
                                .WithElse(elseClause);
                    statement = BuildSyntaxHelpers.AttachWhitespace(statement, nestedElse.Whitespace2Set, WhitespaceLookup);
                }
                var newElseClause = SyntaxFactory.ElseClause(statement);
                newElseClause = BuildSyntaxHelpers.AttachWhitespace(newElseClause, nestedElse.Whitespace2Set, WhitespaceLookup);
                elseClause = newElseClause;
                lastItem = itemAsT;
            }
            return elseClause;
        }

        private ExpressionSyntax GetCondition(IHasCondition itemAsT)
        {
            var expression = (ExpressionSyntax)RDomCSharp.Factory.BuildSyntax(itemAsT.Condition);
            expression = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(expression,
                        itemAsT.Whitespace2Set[LanguageElement.Expression]);
            return expression;
        }

        private StatementSyntax GetStatement(IStatementBlock itemAsT)
        { return RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup); }
    }
}
