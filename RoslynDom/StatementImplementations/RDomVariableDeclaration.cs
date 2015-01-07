using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomVariableDeclaration : RDomBaseVariable
   {
      public RDomVariableDeclaration(string name, string typeName, IExpression initializer = null,
               bool isImplicitlyTyped = false, bool isAliased = false, VariableKind variableKind = VariableKind.Local)
         : base(name, typeName, initializer, isImplicitlyTyped, isAliased, variableKind)
      { }

      public RDomVariableDeclaration(string name, IReferencedType type, IExpression initializer = null,
              bool isImplicitlyTyped = false, bool isAliased = false, VariableKind variableKind = VariableKind.Local)
         : base(name, type, initializer, isImplicitlyTyped, isAliased, variableKind)
      { }

      public RDomVariableDeclaration(SyntaxNode rawItem, IDom parent, SemanticModel model)
         : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
         "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomVariableDeclaration(IVariableDeclaration oldRDom)
         : base(oldRDom)
      { }
   }
}