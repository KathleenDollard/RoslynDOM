using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomCatchStatement : RDomStatementBlockBase<ICatchStatement>, ICatchStatement
   {

      public RDomCatchStatement(string exceptionTypeName = null, IVariableDeclaration variable = null,
                    IExpression condition = null)
            : this(string.IsNullOrWhiteSpace( exceptionTypeName) ? null : new RDomReferencedType(exceptionTypeName),
                           variable,condition )
      {      }

      public RDomCatchStatement(IReferencedType exceptionType = null, IVariableDeclaration variable = null,  
                  IExpression  condition = null)
          : this((SyntaxNode)null, null, null)
      {
         _exceptionType = exceptionType;
         _variable = variable;
         _condition = condition;
      }

      public RDomCatchStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
       "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomCatchStatement(RDomCatchStatement oldRDom)
          : base(oldRDom)
      {
         if (oldRDom.Condition != null)
         { _condition = oldRDom.Condition.Copy(); }
         if (oldRDom.Variable != null)
         { _variable = oldRDom.Variable.Copy(); }
         if (oldRDom.ExceptionType != null)
         { _exceptionType = oldRDom.ExceptionType.Copy(); }
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = new List<IDom>();
            if (this.Variable != null)
            { list.Add(this.Variable); }
            else if (this.ExceptionType != null)
            { list.Add(this.ExceptionType); }
            if (Condition != null)
            { list.Add(Condition); }
            list.AddRange(base.Children.ToList());
            return list;
         }
      }

      private IExpression _condition;
      public IExpression Condition
      {
         get { return _condition; }
         set { SetProperty(ref _condition, value); }
      }

      private IVariableDeclaration _variable;
      public IVariableDeclaration Variable
      {
         get { return _variable; }
         set { SetProperty(ref _variable, value); }
      }

      private IReferencedType _exceptionType;
      public IReferencedType ExceptionType
      {
         get { return _exceptionType; }
         set { SetProperty(ref _exceptionType, value); }
      }
   }
}
