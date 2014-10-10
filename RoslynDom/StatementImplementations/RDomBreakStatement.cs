using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomBreakStatement : RDomBase<IBreakStatement, ISymbol>, IBreakStatement
   {
      public RDomBreakStatement()
      : this(null, null, null)
      {
         NeedsFormatting = true;
      }

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
