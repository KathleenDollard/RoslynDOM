using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    // Doesn't currently follow pattern, ie. a syntax is not passed
    public class RDomTypeParameter : RDomReferencedType, ITypeParameter
    {
        private RDomList<IReferencedType> _constraintTypes;
        public RDomTypeParameter(ImmutableArray<SyntaxReference> raw, ISymbol symbol)
            : base(raw, symbol)
        {
            // DO NOT call Initialize here. It is being called from the base RDomReferencedType class
        }

        public RDomTypeParameter(RDomTypeParameter oldRDom)
             : base(oldRDom)
        {
            _constraintTypes = new RDomList<IReferencedType>(this);
            Variance = oldRDom.Variance;
            Ordinal = oldRDom.Ordinal;
            HasConstructorConstraint = oldRDom.HasConstructorConstraint;
            HasReferenceTypeConstraint = oldRDom.HasReferenceTypeConstraint;
            HasValueTypeConstraint = oldRDom.HasValueTypeConstraint;
            var newConstraints = RoslynDomUtilities.CopyMembers(oldRDom._constraintTypes);
            ConstraintTypes.AddOrMoveRange(newConstraints);
            //foreach (var constraint in newConstraints)
            //{ AddConstraintType(constraint); }
        }

        // TODO: new here feels wrong, so I am currently leaving the warning
        public ITypeParameter Copy()
        {
            return new RDomTypeParameter(this);
        }

        protected override void Initialize()
        {
            base.Initialize();
            _constraintTypes = new RDomList<IReferencedType>(this);
            var typeParamSymbol = Symbol as ITypeParameterSymbol;
            Variance = (Variance)typeParamSymbol.Variance;
            Ordinal = typeParamSymbol.Ordinal;
            HasConstructorConstraint = typeParamSymbol.HasConstructorConstraint;
            HasReferenceTypeConstraint = typeParamSymbol.HasReferenceTypeConstraint;
            HasValueTypeConstraint = typeParamSymbol.HasValueTypeConstraint;
            var constraints = typeParamSymbol.ConstraintTypes;
            foreach (var constraint in constraints)
            { _constraintTypes.AddOrMove(new RDomReferencedType(constraint.DeclaringSyntaxReferences, constraint)); }
        }

        public RDomList<IReferencedType> ConstraintTypes
        { get { return _constraintTypes; } }

        public bool HasConstructorConstraint { get; set; }

        public bool HasReferenceTypeConstraint { get; set; }

        public bool HasValueTypeConstraint { get; set; }

        public int Ordinal { get; set; }

        public Variance Variance { get; set; }

    }
}
