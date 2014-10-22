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
    public class RDomReturnStatementFactory
                : RDomStatementFactory<RDomReturnStatement, ReturnStatementSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomReturnStatementFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.Add(LanguageElement.ReturnKeyword, SyntaxKind.ReturnKeyword);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent,SemanticModel model)
        {
            var syntax = syntaxNode as ReturnStatementSyntax;
            var newItem = new RDomReturnStatement(syntaxNode,parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);


            if (syntax.Expression != null)
            {
                newItem.Return = Corporation.CreateFrom<IExpression>(syntax.Expression, newItem, model).FirstOrDefault();
                Guardian.Assert.IsNotNull(newItem.Return, nameof(newItem.Return));
            }

            return newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IReturnStatement;
            var node = SyntaxFactory.ReturnStatement();
            if (itemAsT.Return != null)
            {
                var returnExpressionSyntax = RDom.CSharp.GetSyntaxNode(itemAsT.Return);
                node = node.WithExpression((ExpressionSyntax)returnExpressionSyntax);
            }

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            return node.PrepareForBuildSyntaxOutput(item);
        }
    }
}
