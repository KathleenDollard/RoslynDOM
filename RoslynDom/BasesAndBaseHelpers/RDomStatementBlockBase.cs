using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomStatementBlockBase<T> : RDomBase<T, ISymbol>, IStatementBlock
       where T : class, IStatementBlock, IDom<T>
   {
      private RDomCollection<IStatementAndDetail> _statements;

      public RDomStatementBlockBase(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      internal RDomStatementBlockBase(T oldRDom)
          : base(oldRDom)
      {
         Initialize();
         _statements = oldRDom.StatementsAll.Copy(this);
         HasBlock = oldRDom.HasBlock;
      }

      private void Initialize()
      {
         _statements = new RDomCollection<IStatementAndDetail>(this);
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = base.Children.ToList();
            list.AddRange(Statements);
            return list;
         }
      }

      private bool _hasBlock;
      public bool HasBlock
      {
         get { return _hasBlock; }
         set { SetProperty(ref _hasBlock, value); }
      }

      public RDomCollection<IStatementAndDetail> StatementsAll
      { get { return _statements; } }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }
   }
}
