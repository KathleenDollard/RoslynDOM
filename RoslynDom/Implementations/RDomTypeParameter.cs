using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomTypeParameter : RDomReferencedType , ITypeParameter 
    {
        internal RDomTypeParameter(ImmutableArray<SyntaxReference> raw, ISymbol symbol)
            : base(raw, symbol)
        {
            Initialize();
        }

        internal RDomTypeParameter(RDomTypeParameter oldRDom)
             : base(oldRDom)
        { }

        // new here feels wrong. Expect testing issues here
        public new ITypeParameter Copy()
        {
            return new RDomTypeParameter(this);
        }

        protected override bool CheckSameIntent(RDomReferencedType  other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            var otherItem = other as RDomTypeParameter;
            if (!base.CheckSameIntent(otherItem, includePublicAnnotations)) return false;
            if (HasConstructorConstraint != otherItem.HasConstructorConstraint) return false;
            if (HasReferenceConstraint != otherItem.HasReferenceConstraint) return false;
            if (HasValueTypeConstraint != otherItem.HasValueTypeConstraint) return false;
            if (Ordinal != otherItem.Ordinal) return false;
            if (Variance != otherItem.Variance) return false;
            return true;
        }
 
        public IEnumerable<IReferencedType> ConstraintTypes
        {
            get
            {
                var symbol = Symbol as ITypeParameterSymbol;
                var constraints = symbol.ConstraintTypes;
                var retList = new List<IReferencedType>();
                foreach (var constraint in constraints)
                {
                   retList.Add( new RDomReferencedType(constraint.DeclaringSyntaxReferences, constraint));
                }
                return retList;
            }
        }

        public bool HasConstructorConstraint
        {
            get
            {
                var symbol = Symbol as ITypeParameterSymbol;
                return symbol.HasConstructorConstraint ;
            }
        }

        public bool HasReferenceConstraint
        {
            get
            {
                var symbol = Symbol as ITypeParameterSymbol;
                return symbol.HasReferenceTypeConstraint ;
            }
        }

        public bool HasValueTypeConstraint
        {
            get
            {
                var symbol = Symbol as ITypeParameterSymbol;
                return symbol.HasValueTypeConstraint ;
            }
        }

        public int Ordinal
        {
            get
            {
                var symbol = Symbol as ITypeParameterSymbol;
                return symbol.Ordinal;
            }
        }

        public Variance Variance
        {
            get
            {
                var symbol = Symbol as ITypeParameterSymbol;
                return (Variance)symbol.Variance;
            }
        }

     }
}
