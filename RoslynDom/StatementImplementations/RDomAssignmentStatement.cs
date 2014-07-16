using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAssignmentStatement : RDomBase<IAssignmentStatement,  ISymbol>, IAssignmentStatement
    {

        internal RDomAssignmentStatement(SyntaxNode rawItem)
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
