using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   // Doesn't currently follow pattern, ie. a syntax is not passed
   public class RDomTypeParameter : RDomBase<ITypeParameter, ISymbol>, ITypeParameter
   {
      private RDomCollection<IReferencedType> _constraintTypes;

      public RDomTypeParameter(string name, int ordinal = 0, bool hasConstructorConstraint = false, 
               bool hasReferenceTypeConstraint = false, bool hasValueTypeConstraint = false, Variance variance = Variance.None)
        : this(null, null, null)
      {
         _name = name;
         _ordinal = ordinal;
         _hasConstructorConstraint = hasConstructorConstraint;
         _hasReferenceTypeConstraint = hasReferenceTypeConstraint;
         _hasValueTypeConstraint = hasValueTypeConstraint;
         _variance = variance;
      }

      public RDomTypeParameter(SyntaxNode rawItem, IDom parent, SemanticModel model)
        : base(rawItem, parent, model)
      { Initialize(); }

      internal RDomTypeParameter(RDomTypeParameter oldRDom)
           : base(oldRDom)
      {
         Initialize();
         _constraintTypes = new RDomCollection<IReferencedType>(this);
         _name = oldRDom.Name;
         _variance = oldRDom.Variance;
         _ordinal = oldRDom.Ordinal;
         _hasConstructorConstraint = oldRDom.HasConstructorConstraint;
         _hasReferenceTypeConstraint = oldRDom.HasReferenceTypeConstraint;
         _hasValueTypeConstraint = oldRDom.HasValueTypeConstraint;
         var newConstraints = RoslynDomUtilities.CopyMembers(oldRDom._constraintTypes);
         ConstraintTypes.AddOrMoveRange(newConstraints);
      }

      protected void Initialize()
      {
         _constraintTypes = new RDomCollection<IReferencedType>(this);
      }

      public RDomCollection<IReferencedType> ConstraintTypes
      { get { return _constraintTypes; } }

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }

      private int _ordinal;
      public int Ordinal
      {
         get { return _ordinal; }
         set { SetProperty(ref _ordinal, value); }
      }

      private bool _hasConstructorConstraint;
      public bool HasConstructorConstraint
      {
         get { return _hasConstructorConstraint; }
         set { SetProperty(ref _hasConstructorConstraint, value); }
      }

      private bool _hasReferenceTypeConstraint;
      public bool HasReferenceTypeConstraint
      {
         get { return _hasReferenceTypeConstraint; }
         set { SetProperty(ref _hasReferenceTypeConstraint, value); }
      }

      private bool _hasValueTypeConstraint;
      public bool HasValueTypeConstraint
      {
         get { return _hasValueTypeConstraint; }
         set { SetProperty(ref _hasValueTypeConstraint, value); }
      }

      private Variance _variance;
      public Variance Variance
      {
         get { return _variance; }
         set { SetProperty(ref _variance, value); }
      }
   }
}
