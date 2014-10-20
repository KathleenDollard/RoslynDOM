using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomDeclarationStatement : RDomBaseVariable, IDeclarationStatement
   {
      public RDomDeclarationStatement(string name, string typeName, IExpression initializer = null,
               bool isImplicitlyTyped = false, bool isAliased = false, bool isConst = false, VariableKind variableKind = VariableKind.Local)
            : this(name, new RDomReferencedType(typeName, true), initializer, isImplicitlyTyped, isAliased, isConst, variableKind)
      { }

      public RDomDeclarationStatement(string name, IReferencedType type, IExpression initializer = null,
              bool isImplicitlyTyped = false, bool isAliased = false, bool isConst = false, VariableKind variableKind = VariableKind.Local)
           : base(name, type, initializer, isImplicitlyTyped, isAliased, variableKind)
      {
         _isConst = IsConst;
      }

      public RDomDeclarationStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomDeclarationStatement(RDomDeclarationStatement oldRDom)
           : base(oldRDom)
      {
         _isConst = oldRDom.IsConst;
      }

      private bool _isConst;
      public bool IsConst
      {
         get { return _isConst; }
         set { SetProperty(ref _isConst, value); }
      }

      IDeclarationStatement IDom<IDeclarationStatement>.Copy()
      {
         return (IDeclarationStatement)base.Copy();
      }

      public override string ToString()
      {
         return base.ToString() + " {" + Type.Name + "}";
      }
   }
}
