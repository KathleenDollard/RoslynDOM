using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomBreakStatement : RDomBase<IBreakStatement, ISymbol>, IBreakStatement
   {
      public RDomBreakStatement()
         : base()
      { }

      public RDomBreakStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomBreakStatement(RDomBreakStatement oldRDom)
         : base(oldRDom)
      { }
   }
}