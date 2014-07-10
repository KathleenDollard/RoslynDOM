using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomTypeParameter : RDomReferencedType, ITypeParameter
    {
        private IList<IReferencedType> _constraintTypes = new List<IReferencedType>();
        internal RDomTypeParameter(ImmutableArray<SyntaxReference> raw, ISymbol symbol)
            : base(raw, symbol)
        {
            // DO NOT call Initialize here. It is being called from the base RDomReferencedType class
        }

        internal RDomTypeParameter(RDomTypeParameter oldRDom)
             : base(oldRDom)
        {
            Variance = oldRDom.Variance;
            Ordinal = oldRDom.Ordinal;
            HasConstructorConstraint = oldRDom.HasConstructorConstraint;
            HasReferenceTypeConstraint = oldRDom.HasReferenceTypeConstraint;
            HasValueTypeConstraint = oldRDom.HasValueTypeConstraint;
            var newConstraints = RoslynDomUtilities.CopyMembers(oldRDom._constraintTypes);
            foreach (var constraint in newConstraints)
            { AddConstraintType(constraint); }
        }

        //public virtual bool Matches(ITypeParameter other)
        //{ return base.Matches(other); }

        // new here feels wrong, so I am currently leaving the warning
        public ITypeParameter Copy()
        {
            return new RDomTypeParameter(this);
        }

        protected override void Initialize()
        {
            base.Initialize();
            var typeParamSymbol = Symbol as ITypeParameterSymbol;
            Variance = (Variance)typeParamSymbol.Variance;
            Ordinal = typeParamSymbol.Ordinal;
            HasConstructorConstraint = typeParamSymbol.HasConstructorConstraint;
            HasReferenceTypeConstraint = typeParamSymbol.HasReferenceTypeConstraint;
            HasValueTypeConstraint = typeParamSymbol.HasValueTypeConstraint;
            var constraints = typeParamSymbol.ConstraintTypes;
            foreach (var constraint in constraints)
            { AddConstraintType(constraint); }
        }

        public IEnumerable<IReferencedType> ConstraintTypes
        { get { return _constraintTypes; } }

        public void AddConstraintType(ITypeSymbol symbol)
        {
            _constraintTypes.Add(new RDomReferencedType(symbol.DeclaringSyntaxReferences, symbol));
        }

        public void AddConstraintType(IReferencedType refType)
        {
            _constraintTypes.Add(refType);
        }

        public void RemoveConstraintType(IReferencedType refType)
        {
            _constraintTypes.Remove(refType);
        }

        public void ClearConstraintTypes()
        {
            _constraintTypes.Clear();
        }

        public bool HasConstructorConstraint { get; set; }

        public bool HasReferenceTypeConstraint { get; set; }

        public bool HasValueTypeConstraint { get; set; }

        public int Ordinal { get; set; }

        public Variance Variance { get; set; }

    }
}
