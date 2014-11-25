using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomUsingStatement : RDomBase<IUsingStatement, ISymbol>, IUsingStatement
   {
      private RDomCollection<IStatementAndDetail> _statements;

      public RDomUsingStatement(IExpression expression, bool hasBlock)
      : this(null, null, null)
      {
         _expression = expression;
         _hasBlock = hasBlock;
      }

      public RDomUsingStatement(IDom parent, IVariableDeclaration variable, bool hasBlock)
      : this(null, parent, null)
      {
         _variable = variable;
         _hasBlock = hasBlock;
      }

      public RDomUsingStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomUsingStatement(RDomUsingStatement oldRDom)
          : base(oldRDom)
      {
         Initialize();
         _statements = oldRDom.StatementsAll.Copy(this);
         _hasBlock = oldRDom.HasBlock;
         if (oldRDom.Expression != null) _expression = oldRDom.Expression.Copy();
         if (oldRDom.Variable != null) _variable = oldRDom.Variable.Copy();
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

      private IExpression _expression;
      public IExpression Expression
      {
         get { return _expression; }
         set { SetProperty(ref _expression, value); }
      }

      private IVariableDeclaration _variable;
      public IVariableDeclaration Variable
      {
         get { return _variable; }
         set { SetProperty(ref _variable, value); }
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
