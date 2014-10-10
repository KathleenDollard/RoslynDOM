using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
 using System.ComponentModel.DataAnnotations;
 using System.ComponentModel.DataAnnotations;namespace RoslynDom
{
    public class RDomVariableDeclaration : RDomBaseVariable
    {
      public RDomVariableDeclaration(string name, string typeName, IExpression initializer = null,
               bool isImplicitlyTyped = false, bool isAliased = false, VariableKind variableKind = VariableKind.Local)
            : this(name, new RDomReferencedType(typeName, true), initializer, isImplicitlyTyped, isAliased, variableKind)
      { }

      public RDomVariableDeclaration(string name, IReferencedType type, IExpression initializer = null,
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
