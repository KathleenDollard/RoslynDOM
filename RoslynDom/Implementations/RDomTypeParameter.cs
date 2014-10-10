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
 public RDomTypeParameter (string  _name,int  _ordinal,bool  _hasConstructorConstraint,bool  _hasReferenceTypeConstraint,bool  _hasValueTypeConstraint,Variance  _variance ) : this (null,null,null ) { Name=_name; Name=_name; Ordinal=_ordinal; Name=_name; Ordinal=_ordinal; HasConstructorConstraint=_hasConstructorConstraint; Name=_name; Ordinal=_ordinal; HasConstructorConstraint=_hasConstructorConstraint; HasReferenceTypeConstraint=_hasReferenceTypeConstraint; Name=_name; Ordinal=_ordinal; HasConstructorConstraint=_hasConstructorConstraint; HasReferenceTypeConstraint=_hasReferenceTypeConstraint; HasValueTypeConstraint=_hasValueTypeConstraint; Name=_name; Ordinal=_ordinal; HasConstructorConstraint=_hasConstructorConstraint; HasReferenceTypeConstraint=_hasReferenceTypeConstraint; HasValueTypeConstraint=_hasValueTypeConstraint; Variance=_variance; }
      public RDomTypeParameter(SyntaxNode rawItem, IDom parent, SemanticModel model)
        : base(rawItem, parent, model)
      { Initialize(); }

      internal RDomTypeParameter(RDomTypeParameter oldRDom)
           : base(oldRDom)
      {
         Initialize();
         _constraintTypes = new RDomCollection<IReferencedType>(this);
         Variance = oldRDom.Variance;
         Ordinal = oldRDom.Ordinal;
         HasConstructorConstraint = oldRDom.HasConstructorConstraint;
         HasReferenceTypeConstraint = oldRDom.HasReferenceTypeConstraint;
         HasValueTypeConstraint = oldRDom.HasValueTypeConstraint;
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
