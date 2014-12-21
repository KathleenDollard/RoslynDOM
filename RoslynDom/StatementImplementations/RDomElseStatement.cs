using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomElseStatement : RDomStatementBlockBase<IFinalElseStatement>, IFinalElseStatement
   {
      public RDomElseStatement()
            : base()
      {  }

      public RDomElseStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomElseStatement(RDomElseStatement oldRDom)
          : base(oldRDom)
      { }
   }
}
