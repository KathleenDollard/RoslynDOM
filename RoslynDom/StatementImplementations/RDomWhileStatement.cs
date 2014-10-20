using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomWhileStatement : RDomBaseLoop<IWhileStatement>, IWhileStatement
   {
      public RDomWhileStatement(IExpression condition, bool testAtEnd = false, bool hasBlock = false)
          : base(condition,  testAtEnd, hasBlock )
      {
      }

      public RDomWhileStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
      {
         TestAtEnd = true;
         Initialize();
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomWhileStatement(RDomWhileStatement oldRDom)
          : base(oldRDom)
      { }
   }
}
