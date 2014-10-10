using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomDoStatement : RDomBaseLoop<IDoStatement>, IDoStatement
    {
      public RDomDoStatement(IExpression condition,bool testAtEnd = false)
          : this(null, null, null)
      {
         NeedsFormatting = true;
         TestAtEnd = testAtEnd;
         Condition = condition;
      }

      public RDomDoStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
        internal RDomDoStatement(RDomDoStatement oldRDom)
            : base(oldRDom)
        { }
    }
}
