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
    public class RDomThrowStatementFactory
                : RDomStatementFactory<RDomThrowStatement, ThrowStatementSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomThrowStatementFactory(RDomCorporation corporation)
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

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ThrowStatementSyntax;
            var newItem = new RDomThrowStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

            if (syntax.Expression != null)
            {
                var expression = Corporation.CreateFrom<IExpression>(syntax.Expression, newItem, model).FirstOrDefault();
                newItem.ExceptionExpression = expression;
            }

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IThrowStatement;
            var node = SyntaxFactory.ThrowStatement();
            var exception = (ExpressionSyntax)RDomCSharp.Factory.BuildSyntax(itemAsT.ExceptionExpression);
            if (exception != null) node = node.WithExpression(exception);

            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);
            return node.PrepareForBuildSyntaxOutput(item);
        }
    }
}
