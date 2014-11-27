using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;

namespace RoslynDom
{
   public class RDomOtherExpression : RDomBaseExpression, IOtherExpression
   {
      public RDomOtherExpression(IDom parent, string initialExpressionString,
               string initialExpressionLanguage, ExpressionType expressionType)
      : base(parent, initialExpressionString, initialExpressionLanguage, expressionType)
      { }

      public RDomOtherExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
        : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomOtherExpression(RDomOtherExpression oldRDom)
          : base(oldRDom)
      { }
   }
}
