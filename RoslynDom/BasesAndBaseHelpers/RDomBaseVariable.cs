using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;

namespace RoslynDom
{
   public abstract class RDomBaseVariable : RDomBase<IVariableDeclaration, ISymbol>, IVariableDeclaration
   {
      protected RDomBaseVariable( string name, string typeName, IExpression initializer, bool isImplicitlyTyped, bool isAliased, VariableKind variableKind)
          : this(name, initializer, isImplicitlyTyped, isAliased, variableKind)
      {
         _type = new RDomReferencedType(this, typeName, isAliased);
      }

      protected RDomBaseVariable( string name, IReferencedType type, IExpression initializer, bool isImplicitlyTyped, bool isAliased, VariableKind variableKind)
          : this( name, initializer, isImplicitlyTyped, isAliased, variableKind )
      {
         _type = type;
      }

      private RDomBaseVariable( string name,  IExpression initializer, bool isImplicitlyTyped, bool isAliased, VariableKind variableKind)
         : base()
      {
         _name = name;
         _initializer = initializer;
         _isImplicitlyTyped = IsImplicitlyTyped;
         _isAliased = isAliased;
         _variableKind = variableKind;
      }

      protected RDomBaseVariable(SyntaxNode rawItem, IDom parent, SemanticModel model)
          : base(rawItem, parent, model)
      { }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
      "CA1811:AvoidUncalledPrivateCode", Justification = "Called via Reflection")]
      internal RDomBaseVariable(IVariableDeclaration oldRDom)
          : base(oldRDom)
      {
         _name = oldRDom.Name;
         _type = oldRDom.Type.Copy();
         _isImplicitlyTyped = oldRDom.IsImplicitlyTyped;
         _isAliased = oldRDom.IsAliased;
         _variableKind = oldRDom.VariableKind;
         if (oldRDom.Initializer != null) _initializer = oldRDom.Initializer.Copy();
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
