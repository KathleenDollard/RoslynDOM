using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using cm=System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomAssignmentStatement : RDomBase<IAssignmentStatement, ISymbol>, IAssignmentStatement
   {

      /// <summary>
      /// Constructor to use when creating a RoslynDom from scratch
      /// </summary>
      /// <param name="left">
      /// Expression to assign to
      /// </param>
      /// <param name="expression">
      /// Expression to assign
      /// </param>
      /// <param name="op">
      /// Assignment operator
      /// </param>
      public RDomAssignmentStatement(IExpression left, IExpression expression, AssignmentOperator op = AssignmentOperator.Equals)
       : this(null, null, null)
      {
         Left = left;
         Expression = expression;
         Operator = op;
      }

      [cm.EditorBrowsable(cm.EditorBrowsableState.Never)]
      public RDomAssignmentStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
            : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomAssignmentStatement(RDomAssignmentStatement oldRDom)
          : base(oldRDom)
      {
         Left = oldRDom.Left.Copy();
         Expression = oldRDom.Expression.Copy();
         Operator = oldRDom.Operator;
      }

      public override IEnumerable<IDom> Children
      { get { return new List<IDom>() { Expression }; } }

      private IExpression _left;
      [Required]
      public IExpression Left
      {
         get { return _left; }
         set { SetProperty(ref _left, value); }
      }
      private IExpression _expression;
      [Required]
      public IExpression Expression
      {
         get { return _expression; }
         set { SetProperty(ref _expression, value); }
      }
      public AssignmentOperator Operator
      {
         get { return _operator; }
         set { SetProperty(ref _operator, value); }
      }
      private AssignmentOperator _operator;
   }
}
