using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface IAssignmentStatement : IStatement, IDom<IAssignmentStatement >
    {
        IExpression Expression { get; set; }
        IExpression Left { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
           "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Operator",
          Justification = "Because this represents an operator, it's seems an appropriate name")]
        AssignmentOperator Operator { get; set; }

    }
}
