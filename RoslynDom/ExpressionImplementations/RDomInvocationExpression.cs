using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;

namespace RoslynDom
{
   public class RDomInvocationExpression : RDomBaseExpression, IInvocationExpression
   {
      private RDomCollection<IReferencedType> _typeArguments;
      private RDomCollection<IArgument> _arguments;

      public RDomInvocationExpression(IDom parent, string initialExpressionString,
               string initialExpressionLanguage, string methodName)
         : base(parent, initialExpressionString, initialExpressionLanguage, ExpressionType.Invocation)
      {
         _methodName = methodName;
      }

      public RDomInvocationExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      {
         Initialize();
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
      "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomInvocationExpression(RDomInvocationExpression oldRDom)
         : base(oldRDom)
      {
         _typeArguments = oldRDom.TypeArguments.Copy(this);
         _arguments = oldRDom.Arguments.Copy(this);
      }

      private void Initialize()
      {
         _typeArguments = new RDomCollection<IReferencedType>(this);
         _arguments = new RDomCollection<IArgument>(this);
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