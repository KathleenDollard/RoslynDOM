using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;

namespace RoslynDom
{
   public class RDomLambdaSingleExpression : RDomBaseExpression, ILambdaSingleExpression
   {
      private RDomCollection<IParameter> _parameters;

      public RDomLambdaSingleExpression(IDom parent, string initialExpressionString,
               string initialExpressionLanguage)
         : base(parent, initialExpressionString, initialExpressionLanguage, ExpressionType.Lambda)
      { }

      public RDomLambdaSingleExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
      "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomLambdaSingleExpression(RDomLambdaSingleExpression oldRDom)
         : base(oldRDom)
      {
         _parameters = oldRDom.Parameters.Copy(this);
      }

      private void Initialize()
      {
         _parameters = new RDomCollection<IParameter>(this);
      }

      private IExpression _expression;

      [Required]
      public IExpression Expression
      {
         get { return _expression; }
         set { SetProperty(ref _expression, value); }
      }

      private IReferencedType _returnType;

      [Required]
      public IReferencedType ReturnType
      {
         get { return _returnType; }
         set { SetProperty(ref _returnType, value); }
      }

      public RDomCollection<IParameter> Parameters
      { get { return _parameters; } }
   }
}