using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;

namespace RoslynDom
{
   public class RDomExpression : RDomBase<IExpression, ISymbol>, IExpression
   {
      public RDomExpression(string expression, ExpressionType expressionType)
      : this(null, null, null)
      {
         NeedsFormatting = true;
         Expression = expression;
         ExpressionType = expressionType;
      }

      public RDomExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
        : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomExpression(RDomExpression oldRDom)
          : base(oldRDom)
      {
         Expression = oldRDom.Expression;
         ExpressionType = oldRDom.ExpressionType;
      }

      private string _expression;
      [Required]
      public string Expression
      {
         get { return _expression; }
         set { SetProperty(ref _expression, value); }
      }

      private ExpressionType _expressionType;
     [Required]
      public ExpressionType ExpressionType
      {
         get { return _expressionType; }
         set { SetProperty(ref _expressionType, value); }
      }
   }
}
