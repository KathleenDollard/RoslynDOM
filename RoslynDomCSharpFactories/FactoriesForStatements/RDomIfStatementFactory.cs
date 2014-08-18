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
                    _whitespaceLookup.Add(LanguageElement.IfKeyword, SyntaxKind.IfKeyword );
                    _whitespaceLookup.Add(LanguageElement.ConditionalStartDelimiter , SyntaxKind.OpenParenToken );
                    _whitespaceLookup.Add(LanguageElement.ConditionalEndDelimiter, SyntaxKind.CloseParenToken );
                    _whitespaceLookup.Add(LanguageElement.StatementBlockStartDelimiter, SyntaxKind.OpenBraceToken);
                    _whitespaceLookup.Add(LanguageElement.StatementBlockEndDelimiter , SyntaxKind.CloseBraceToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as IfStatementSyntax;
            var newItem = new RDomIfStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);
            newItem.Condition = Corporation.CreateFrom<IExpression>(syntax.Condition, newItem, model).FirstOrDefault();
            CreateFromWorker.InitializeStatements(newItem, syntax.Statement, newItem, model);

            CreateFromWorker.StoreWhitespace(newItem, syntax.Statement, LanguagePart.Block, WhitespaceLookup);
            CreateFromWorker.StoreWhitespace(newItem, syntax.Condition, LanguagePart.Current, WhitespaceLookup);

            var elseIfSyntaxList = GetElseIfSyntaxList(syntax);
            foreach (var elseIf in elseIfSyntaxList.Skip(1))  // The first is the root if
            {
                var newElse = new RDomElseIfStatement(elseIf, newItem, model);
                CreateFromWorker.StandardInitialize(newElse, syntaxNode, newElse, model);
                CreateFromWorker.InitializeStatements(newElse, elseIf.Statement, newElse, model);
                newElse.Condition = Corporation.CreateFrom<IExpression>(elseIf.Condition, newElse, model).FirstOrDefault();
                newItem.Elses.AddOrMove(newElse);
            }
            var lastElseIf = elseIfSyntaxList.Last();
            if (lastElseIf.Else != null && lastElseIf.Else.Statement != null)
            {
                var newElse = new RDomElseStatement(syntax, newItem, model);
                CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
                CreateFromWorker.InitializeStatements(newElse, lastElseIf.Else.Statement, newElse, model);
                newItem.Elses.AddOrMove(newElse);
            }
            return newItem;
        }


        private IEnumerable<IfStatementSyntax> GetElseIfSyntaxList(IfStatementSyntax syntax)
        {
            // You can't use descendants here becuase it is a very specific pattern 
            var list = new List<IfStatementSyntax>();
            list.Add(syntax);
            if (syntax.Else != null)
            {
                var elseAsIf = syntax.Else.Statement as IfStatementSyntax;
                if (elseAsIf == null) { } // At terminus of recursion
                else
                {
                    // Recurse down the if chain
                    list.AddRange(GetElseIfSyntaxList(elseAsIf));
                }
            }
            return list;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IIfStatement;
            var elseSyntax = BuildElseSyntax(itemAsT.Elses);
            var node = SyntaxFactory.IfStatement(GetCondition(itemAsT), GetStatement(itemAsT));
            if (elseSyntax != null) { node = node.WithElse(elseSyntax); }

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            return node.PrepareForBuildSyntaxOutput(item);
        }

        private ElseClauseSyntax BuildElseSyntax(IEnumerable<IElseStatement> elses)
        {
            // Because we reversed the list, inner is first, inner to outer required for this approach
            elses = elses.Reverse();
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
                }
                var newElseClause = SyntaxFactory.ElseClause(statement);
                elseClause = newElseClause;
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
        { return RoslynCSharpUtilities.BuildStatement(itemAsT.Statements, itemAsT, WhitespaceLookup);        }
    }
}
