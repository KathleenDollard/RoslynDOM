using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomInvocationStatementFactory
         : RDomStatementFactory<RDomInvocationStatement, ExpressionStatementSyntax>
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            var itemAsT = item as IInvocationStatement;
            var expressionSyntax = RDomFactory.BuildSyntax(itemAsT.Invocation);
            var node = SyntaxFactory.ExpressionStatement((ExpressionSyntax)expressionSyntax);
            return new SyntaxNode[] { RoslynUtilities.Format(node) };

        }

        public override FactoryPriority Priority
        {
            get { return FactoryPriority.Normal - 1; }
        }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return syntaxNode is ExpressionStatementSyntax;
        }
    }


    public class RDomInvocationStatement : RDomBase<IInvocationStatement, ExpressionStatementSyntax, ISymbol>, IInvocationStatement
    {

        internal RDomInvocationStatement(ExpressionStatementSyntax rawItem)
           : base(rawItem)
        {
            Initialize2();
        }

        internal RDomInvocationStatement(
              ExpressionStatementSyntax rawExpression,
              IEnumerable<PublicAnnotation> publicAnnotations)
            : base(rawExpression, publicAnnotations)
        {
            Initialize();
        }

        internal RDomInvocationStatement(RDomInvocationStatement oldRDom)
             : base(oldRDom)
        {
            Invocation = oldRDom.Invocation.Copy();
        }

        protected override void Initialize()
        {
            base.Initialize();
            var expression = TypedSyntax.Expression;
            Invocation = RDomFactoryHelper.ExpressionFactoryHelper.MakeItem(expression).FirstOrDefault();
        }

        protected void Initialize2()
        {
            Initialize();
        }

        public override ExpressionStatementSyntax BuildSyntax()
        {
            return null;
        }

        public IExpression Invocation { get; set; }
    }
}
