using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public abstract class RDomBaseVariable : RDomBase<IVariableDeclaration, ISymbol>, IVariableDeclaration
   {
      protected RDomBaseVariable(SyntaxNode rawItem, IDom parent, SemanticModel model)
          : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
      "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomBaseVariable(IVariableDeclaration oldRDom)
          : base(oldRDom)
      {
         VariableKind = oldRDom.VariableKind;
         IsImplicitlyTyped = oldRDom.IsImplicitlyTyped;
         IsAliased = oldRDom.IsAliased;
         Type = oldRDom.Type.Copy();
         if (oldRDom.Initializer != null) Initializer = oldRDom.Initializer.Copy();
      }

      public override IEnumerable<IDom> Children
      {
         get
         {
            var list = new List<IDom>();
            if (Initializer != null)
            { list.Add(Initializer); }
            return list;
         }
      }

      [Required]
      public string Name { get; set; }
      [Required]
      public IReferencedType Type { get; set; }
      public IExpression Initializer { get; set; }
      public bool IsImplicitlyTyped { get; set; }
      public bool IsAliased { get; set; }
      public VariableKind VariableKind { get; set; }
   }
}
