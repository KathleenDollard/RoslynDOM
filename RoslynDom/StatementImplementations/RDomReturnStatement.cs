using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomReturnStatement : RDomBase<IReturnStatement, ReturnStatementSyntax, ISymbol>, IReturnStatement
    {

        internal RDomReturnStatement(ReturnStatementSyntax rawItem)
           : base(rawItem)
        {
           // Initialize2();
        }

        //internal RDomReturnStatement(
        //      ReturnStatementSyntax rawReturn,
        //      IEnumerable<PublicAnnotation> publicAnnotations)
        //    : base(rawReturn, publicAnnotations)
        //{
        //    Initialize();
        //}

        internal RDomReturnStatement(RDomReturnStatement oldRDom)
             : base(oldRDom)
        {
            Return = oldRDom.Return.Copy();
        }

        //protected override void Initialize()
        //{
        //    base.Initialize();
        //    if (TypedSyntax.Expression != null)
        //    {
        //        Return = RDomFactoryHelper.ExpressionFactoryHelper.MakeItem(TypedSyntax.Expression).FirstOrDefault();
        //        if (Return == null) throw new InvalidOperationException();
        //    }
        //}

        //protected void Initialize2()
        //{
        //    Initialize();
        //}

        //public override ReturnStatementSyntax BuildSyntax()
        //{
        //    var expr = ((RDomExpression)Return).BuildSyntax();
        //    var node = SyntaxFactory.ReturnStatement(expr);
        //    return node;
        //}

        public IExpression Return { get; set; }
    }
}
