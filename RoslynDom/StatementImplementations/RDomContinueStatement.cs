using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomContinueStatement : RDomBase<IContinueStatement, ISymbol>, IContinueStatement
   {
      public RDomContinueStatement()
         : base()
      { }

      public RDomContinueStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomContinueStatement(RDomContinueStatement oldRDom)
         : base(oldRDom)
      { }
   }
}