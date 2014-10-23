using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Linq;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomBlockStatement : RDomBase<IBlockStatement, ISymbol>, IBlockStatement
   {
      private RDomCollection<IStatementAndDetail> _statements;

      public RDomBlockStatement()
      : this(null, null, null)
      {
      }

      public RDomBlockStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomBlockStatement(RDomBlockStatement oldRDom)
          : base(oldRDom)
      {
         _statements = oldRDom.Statements.Copy(this);
      }

      protected void Initialize()
      {
         _statements = new RDomCollection<IStatementAndDetail>(this);
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = base.Children.ToList();
            list.AddRange(_statements);
            return list;
         }
      }

      public RDomCollection<IStatementAndDetail> Statements
      { get { return _statements; } }
   }
}
