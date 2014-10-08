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
      private RDomCollection<IStatementCommentWhite> _statements;

      internal RDomBaseLoop(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      {
         Initialize();
      }

      internal RDomBaseLoop(T oldRDom)
           : base(oldRDom)
      {
         Initialize();
         var statements = RoslynDomUtilities.CopyMembers(oldRDom.Statements);
         StatementsAll.AddOrMoveRange(statements);
         Condition = oldRDom.Condition.Copy();
         HasBlock = oldRDom.HasBlock;
         TestAtEnd = oldRDom.TestAtEnd;
      }

      protected void Initialize()
      {
         _statements = new RDomCollection<IStatementCommentWhite>(this);
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

      [Required]
      public IExpression Condition { get; set; }
      public bool TestAtEnd { get; set; }
      public bool HasBlock { get; set; }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }

      public RDomCollection<IStatementCommentWhite> StatementsAll
      { get { return _statements; } }
   }
}