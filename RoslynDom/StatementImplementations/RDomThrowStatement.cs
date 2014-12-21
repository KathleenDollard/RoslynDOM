using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomThrowStatement : RDomBase<IThrowStatement, ISymbol>, IThrowStatement
   {
      public RDomThrowStatement(IExpression exceptionExpression = null)
            : base()
      {
         _exceptionExpression = exceptionExpression;
      }

      public RDomThrowStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomThrowStatement(RDomThrowStatement oldRDom)
          : base(oldRDom)
      {
         _exceptionExpression = oldRDom.ExceptionExpression.Copy();
      }

      private IExpression _exceptionExpression;
      public IExpression ExceptionExpression
      {
         get { return _exceptionExpression; }
         set { SetProperty(ref _exceptionExpression, value); }
      }
   }
}
