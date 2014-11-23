using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace RoslynDom
{
   public class RDomInvocationExpression : RDomBase<IExpression, ISymbol>, IInvocationExpression
   {
      private RDomCollection<IReferencedType> _typeArguments;
      private RDomCollection<IArgument > _arguments;

      public RDomInvocationExpression(string expression)
      : this(null, null, null)
      {
         NeedsFormatting = true;
         Expression = expression;
         ExpressionType = ExpressionType.Invocation;
      }

      public RDomInvocationExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
        : base(rawItem, parent, model)
      { Initialize(); }

       [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomInvocationExpression(RDomInvocationExpression oldRDom)
          : base(oldRDom)
      {
         Initialize();
         Expression = oldRDom.Expression;
         ExpressionType = oldRDom.ExpressionType;
      }

     private void Initialize()
      {
         _typeArguments = new RDomCollection<IReferencedType>(this);
         _arguments = new RDomCollection<IArgument>(this);
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
         set {  } // do nothing
      }

      private string _methodName;
      [Required]
      public string MethodName
      {
         get { return _methodName; }
         set { SetProperty(ref _methodName, value); }
      }

      public RDomCollection<IReferencedType> TypeArguments
      { get { return _typeArguments; } }

      public RDomCollection<IArgument> Arguments
      { get { return _arguments; } }

   }
}
