using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAssignmentStatementFactory
         : RDomStatementFactory<RDomAssignmentStatement, ExpressionStatementSyntax>
    {
        public override void InitializeItem(RDomAssignmentStatement newItem, ExpressionStatementSyntax syntax)
        {
            var binary = syntax.Expression as BinaryExpressionSyntax;
            if (binary == null) throw new InvalidOperationException();
            // TODO: handle all the other kinds of assigments here (like +=)
            if (binary.CSharpKind() != SyntaxKind.SimpleAssignmentExpression) { throw new NotImplementedException(); }
            var left = binary.Left;
            var identifier = left as IdentifierNameSyntax;
            if (identifier == null) throw new InvalidOperationException();
            var right = binary.Right;
            var expression = right as ExpressionSyntax;
            if (expression == null) throw new InvalidOperationException();
            newItem.Name = identifier.ToString();
            newItem.Expression = RDomFactoryHelper.ExpressionFactoryHelper.MakeItem(expression).FirstOrDefault();
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var nameSyntax = SyntaxFactory.IdentifierName(item.Name);
            var itemAsT = item as IAssignmentStatement;
            var expressionSyntax = RDomFactory.BuildSyntax(itemAsT.Expression );

            var assignmentSyntax = SyntaxFactory.BinaryExpression(SyntaxKind.SimpleAssignmentExpression, nameSyntax, (ExpressionSyntax)expressionSyntax);
            var node = SyntaxFactory.ExpressionStatement(assignmentSyntax );
            return new SyntaxNode[] { RoslynUtilities.Format(node) };

        }
        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            var statement = syntaxNode as ExpressionStatementSyntax;
            if (statement == null) { return false; }
            return statement.Expression.CSharpKind() == SyntaxKind.SimpleAssignmentExpression;
        }
    }


    public class RDomAssignmentStatement : RDomBase<IAssignmentStatement, ExpressionStatementSyntax, ISymbol>, IAssignmentStatement
    {

        internal RDomAssignmentStatement(ExpressionStatementSyntax rawItem)
           : base(rawItem)
        {
            //Initialize2();
        }

        //internal RDomAssignmentStatement(
        //      ExpressionStatementSyntax rawExpression,
        //      IEnumerable<PublicAnnotation> publicAnnotations)
        //    : base(rawExpression,  publicAnnotations)
        //{
        //    Initialize();
        //}

        internal RDomAssignmentStatement(RDomAssignmentStatement oldRDom)
             : base(oldRDom)
        {
            VarName = oldRDom.VarName;
            Expression = oldRDom.Expression;
        }

        //protected override void Initialize()
        //{
        //    base.Initialize();
        //    var binary = TypedSyntax.Expression as BinaryExpressionSyntax;
        //    if (binary == null) throw new InvalidOperationException();
        //    // TODO: handle all the other kinds of assigments here (like +=)
        //    if (binary.CSharpKind() != SyntaxKind.SimpleAssignmentExpression ) { throw  new NotImplementedException(); }
        //    var left = binary.Left;
        //    var identifier = left as IdentifierNameSyntax;
        //    if (identifier == null) throw new InvalidOperationException();
        //    var right = binary.Right;
        //    var expression = right as ExpressionSyntax;
        //    if (expression == null) throw new InvalidOperationException();
        //    Name = identifier.ToString();
        //    Expression = RDomFactoryHelper.ExpressionFactoryHelper.MakeItem(expression).FirstOrDefault();
        //}

        //protected void Initialize2()
        //{
        //    Initialize();
        //}

        //public override ExpressionStatementSyntax BuildSyntax()
        //{
        //    return null;
        //}

        public string VarName { get; set; }
        public IExpression Expression { get; set; }
    }
}
