using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomInvocationStatementFactory
         : RDomStatementFactory<RDomInvocationStatement, ExpressionStatementSyntax>
    {
        public RDomInvocationStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

           public override RDomPriority Priority
        { get { return RDomPriority.Normal - 1; } }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {  // Restates the obvious to clarify this is the default
            return syntaxNode is ExpressionStatementSyntax;
        }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ExpressionStatementSyntax;
            var newItem = new RDomInvocationStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.StoreWhitespaceForFirstAndLastToken(newItem, syntax, LanguagePart.Current,
                                    LanguageElement.Expression);

            var expression = syntax.Expression;
            newItem.Invocation = Corporation.CreateFrom<IExpression>(expression, newItem, model).FirstOrDefault();

            return newItem;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IInvocationStatement;
            var expressionSyntax = RDom.CSharp.GetSyntaxNode(itemAsT.Invocation);
            var node = SyntaxFactory.ExpressionStatement((ExpressionSyntax)expressionSyntax);

            node = BuildSyntaxHelpers.AttachWhitespaceToFirstAndLast(node,
                        itemAsT.Whitespace2Set[LanguageElement.Expression]);
            return node.PrepareForBuildSyntaxOutput(item);
        }


    }
}
