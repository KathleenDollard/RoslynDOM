using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public abstract class RDomBaseLoop<T>
       : RDomBase<T, ISymbol>, IStatement
       where T : class, ILoop<T>, IStatement
   {
      private RDomCollection<IStatementAndDetail> _statements;

      protected RDomBaseLoop( IExpression condition, bool testAtEnd, bool hasBlock)
          : base()
      {
         Initialize();
         _condition = condition;
         _testAtEnd = testAtEnd;
         _hasBlock = hasBlock;
      }

      internal RDomBaseLoop(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      {
         Initialize();
      }

      internal RDomBaseLoop(T oldRDom)
           : base(oldRDom)
      {
         Initialize();
         _statements = oldRDom.StatementsAll.Copy(this);
         _condition = oldRDom.Condition == null ? null : oldRDom.Condition.Copy();
         _testAtEnd = oldRDom.TestAtEnd;
         _hasBlock = oldRDom.HasBlock;
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
            list.Add(Condition);
            list.AddRange(Statements);
            return list;
         }
      }

      private IExpression _condition;
      [Required]
      public IExpression Condition
      {
         get { return _condition; }
         set { SetProperty(ref _condition, value); }
      }

      private bool _testAtEnd;
      public bool TestAtEnd
      {
         get { return _testAtEnd; }
         set { SetProperty(ref _testAtEnd, value); }
      }

      private bool _hasBlock;
      public bool HasBlock
      {
         get { return _hasBlock; }
         set { SetProperty(ref _hasBlock, value); }
      }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }

      public RDomCollection<IStatementAndDetail> StatementsAll
      { get { return _statements; } }
   }
}