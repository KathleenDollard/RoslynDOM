using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;

namespace RoslynDom
{
   public class RDomExpression : RDomBase<IExpression, ISymbol>, IExpression
   {
      public RDomExpression(string initialExpressionString, 
               string initialExpressionLanguage, ExpressionType expressionType)
      : this(null, null, null)
      {
         NeedsFormatting = true;
         InitialExpressionString = initialExpressionString;
         InitialExpressionLanguage = initialExpressionLanguage;
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
         InitialExpressionString = oldRDom.InitialExpressionString;
         InitialExpressionLanguage = oldRDom.InitialExpressionLanguage;
         ExpressionType = oldRDom.ExpressionType;
      }

      private string _intialExpressionString;
      [Required]
      public string InitialExpressionString
      {
         get { return _intialExpressionString; }
         set { SetProperty(ref _intialExpressionString, value); }
      }

      private string _initialExpressionLanguage;
      [Required]
      public string InitialExpressionLanguage
      {
         get { return _initialExpressionLanguage; }
         set { SetProperty(ref _initialExpressionLanguage, value); }
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
