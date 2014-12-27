using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom
{
   public class RDomLambdaMultiLineExpression : RDomBaseExpression, ILambdaMultiLineExpression
   {
      private RDomCollection<IParameter> _parameters;
      private RDomCollection<IStatementAndDetail> _statements;

      public RDomLambdaMultiLineExpression(SyntaxNode rawItem, IDom parent, SemanticModel model)
        : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
      "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomLambdaMultiLineExpression(RDomLambdaMultiLineExpression oldRDom)
         : base(oldRDom)
      {
         _parameters = oldRDom.Parameters.Copy(this);
         _statements = oldRDom.StatementsAll.Copy(this);
      }

      private void Initialize()
      {
         _parameters = new RDomCollection<IParameter>(this);
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

      private IReferencedType _returnType;
      [Required]
      public IReferencedType ReturnType
      {
         get { return _returnType; }
         set { SetProperty(ref _returnType, value); }
      }

      public RDomCollection<IParameter> Parameters
      { get { return _parameters; } }

      public RDomCollection<IStatementAndDetail> StatementsAll
      { get { return _statements; } }

      public IEnumerable<IStatement> Statements
      { get { return _statements.OfType<IStatement>().ToList(); } }

      public bool HasBlock
      {
         get { return true; }
         set { }
      }
   }
}
