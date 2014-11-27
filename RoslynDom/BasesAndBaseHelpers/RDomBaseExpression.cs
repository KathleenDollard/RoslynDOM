using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;

namespace RoslynDom
{
   public abstract class RDomBaseExpression : RDomBase<IExpression, ISymbol>
   {
      protected RDomBaseExpression(IDom parent, string initialExpressionString, 
               string initialExpressionLanguage, ExpressionType expressionType)
      : this(null, parent, null)
      {
         NeedsFormatting = true;
         InitialExpressionString = initialExpressionString;
         InitialExpressionLanguage = initialExpressionLanguage;
         ExpressionType = expressionType;
      }

      protected RDomBaseExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
        : base(rawItem, parent, model)
      { }

      protected RDomBaseExpression(IExpression oldRDom)
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
