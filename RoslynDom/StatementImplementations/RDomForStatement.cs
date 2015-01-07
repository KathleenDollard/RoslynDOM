using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.Collections.Generic;

namespace RoslynDom
{
   public class RDomForStatement : RDomBaseLoop<IForStatement>, IForStatement
   {
      public RDomForStatement(IVariableDeclaration variable, IExpression condition, IExpression incrementor, bool hasBlock)
         : base(condition, false, hasBlock)
      {
         _incrementor = incrementor;
         _variable = variable;
      }

      public RDomForStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { Initialize(); }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
          "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomForStatement(RDomForStatement oldRDom)
         : base(oldRDom)
      {
         _incrementor = oldRDom.Incrementor.Copy();
         _variable = oldRDom.Variable.Copy();
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = new List<IDom>();
            list.Add(Incrementor);
            list.Add(Variable);
            list.AddRange(base.Children);
            return list;
         }
      }

      private IExpression _incrementor;

      public IExpression Incrementor
      {
         get { return _incrementor; }
         set { SetProperty(ref _incrementor, value); }
      }

      public IVariableDeclaration Variable
      {
         get { return _variable; }
         set { SetProperty(ref _variable, value); }
      }

      private IVariableDeclaration _variable;
   }
}