using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomAssignmentStatementFactory
         : RDomStatementFactory<RDomAssignmentStatement, ExpressionStatementSyntax>
    {
        public RDomAssignmentStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        public override RDomPriority Priority
        { get { return RDomPriority.Normal + 1; } }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            var statement = syntaxNode as ExpressionStatementSyntax;
            if (statement == null) { return false; }
            return (statement.Expression is BinaryExpressionSyntax);
        }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ExpressionStatementSyntax;
            var newItem = new RDomAssignmentStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            var binary = syntax.Expression as BinaryExpressionSyntax;
            Guardian.Assert.IsNotNull(binary, nameof(binary));
            var left = binary.Left as ExpressionSyntax;
            // Previously tested for identifier here, but can also be SimpleMemberAccess and ElementAccess expressions
            // not currently seeing value in testing for the type. Fix #46
            // Also changed Name to Left and string to expression
            var right = binary.Right;
            var expression = right as ExpressionSyntax;
            Guardian.Assert.IsNotNull(expression, nameof(expression));
            newItem.Left = Corporation.CreateFrom<IExpression>(left, newItem, model).FirstOrDefault();
            newItem.Expression = Corporation.CreateFrom<IExpression>(expression, newItem, model).FirstOrDefault();
            newItem.Operator = Mappings.OperatorFromCSharpKind (binary.CSharpKind());
            return newItem;
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IAssignmentStatement;
            var leftSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Left);
            var expressionSyntax = RDomCSharp.Factory.BuildSyntax(itemAsT.Expression);
            var syntaxKind =Mappings. SyntaxKindFromOperator (itemAsT.Operator);
            var assignmentSyntax = SyntaxFactory.BinaryExpression(syntaxKind,
                            (ExpressionSyntax)leftSyntax, (ExpressionSyntax)expressionSyntax);
            var node = SyntaxFactory.ExpressionStatement(assignmentSyntax);

            return item.PrepareForBuildSyntaxOutput(node);

        }

    

    }
}
