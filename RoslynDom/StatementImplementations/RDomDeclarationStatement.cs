using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomDeclarationStatement : RDomBaseVariable, IDeclarationStatement
   {
      public RDomDeclarationStatement(string name, string typeName, IExpression initializer = null,
               bool isImplicitlyTyped = false, bool isAliased = false, VariableKind variableKind = VariableKind.Local)
            : this(name, new RDomReferencedType(typeName, true), initializer, isImplicitlyTyped, isAliased, variableKind)
      { }

      public RDomDeclarationStatement(string name, IReferencedType type, IExpression initializer = null,
              bool isImplicitlyTyped = false, bool isAliased = false, VariableKind variableKind = VariableKind.Local)
           : this((SyntaxNode)null, null, null)
      {
         Name = name;
         Type = type;
         Initializer = initializer;
         IsImplicitlyTyped = isImplicitlyTyped;
         IsAliased = isAliased;
         VariableKind = variableKind;
      }

      public RDomDeclarationStatement(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomDeclarationStatement(RDomDeclarationStatement oldRDom)
           : base(oldRDom)
      {
         IsConst = oldRDom.IsConst;
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
