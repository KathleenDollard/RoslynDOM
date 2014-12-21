using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomTryStatement : RDomStatementBlockBase<ITryStatement>, ITryStatement
   {
      private RDomCollection<ICatchStatement> _catches;

      public RDomTryStatement(IFinallyStatement finallyStatement = null)
            : base()
      {
         Initialize();
         _finally = finallyStatement;
      }

      public RDomTryStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomTryStatement(RDomTryStatement oldRDom)
          : base(oldRDom)
      {
         _catches = oldRDom.CatchesAll.Copy(this);
         _finally = oldRDom.Finally.Copy();
      }

      private void Initialize()
      {
         _catches = new RDomCollection<ICatchStatement>(this);
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = new List<IDom>();
            list.AddRange(base.Children.ToList());
            list.AddRange(Catches);
            list.Add(Finally);
            return list;
         }
      }

      public RDomCollection<ICatchStatement> CatchesAll
      { get { return _catches; } }

      public IEnumerable<ICatchStatement> Catches
      { get { return _catches; } }

      private IFinallyStatement _finally;
      public IFinallyStatement Finally
      {
         get { return _finally; }
         set { SetProperty(ref _finally, value); }
      }
   }
}
