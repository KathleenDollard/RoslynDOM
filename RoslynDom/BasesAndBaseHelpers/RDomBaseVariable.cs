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

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      private IReferencedType _type;
      [Required]
      public IReferencedType Type
      {
         get { return _type; }
         set { SetProperty(ref _type, value); }
      }

      private IExpression _initializer;
      public IExpression Initializer
      {
         get { return _initializer; }
         set { SetProperty(ref _initializer, value); }
      }

      private bool _isImplicitlyTyped;
      public bool IsImplicitlyTyped
      {
         get { return _isImplicitlyTyped; }
         set { SetProperty(ref _isImplicitlyTyped, value); }
      }

      private bool _isAliased;
      public bool IsAliased
      {
         get { return _isAliased; }
         set { SetProperty(ref _isAliased, value); }
      }

      private VariableKind _variableKind;
      public VariableKind VariableKind
      {
         get { return _variableKind; }
         set { SetProperty(ref _variableKind, value); }
      }
   }
}
