using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    // Doesn't currently follow pattern, ie. a syntax is not passed
    public class RDomTypeParameter : RDomBase<ITypeParameter, ISymbol>, ITypeParameter
    {
        private RDomCollection<IReferencedType> _constraintTypes;

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

        protected  void Initialize()
        {
            _constraintTypes = new RDomCollection<IReferencedType>(this);
        }


        public RDomCollection<IReferencedType> ConstraintTypes
        { get { return _constraintTypes; } }

        public bool HasConstructorConstraint { get; set; }

        public bool HasReferenceTypeConstraint { get; set; }

        public bool HasValueTypeConstraint { get; set; }

        public int Ordinal { get; set; }

        public Variance Variance { get; set; }

        public string Name { get; set; }
    }
}
