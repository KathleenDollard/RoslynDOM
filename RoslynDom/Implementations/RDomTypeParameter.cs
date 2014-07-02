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
        { }

        internal RDomTypeParameter(RDomTypeParameter oldRDom)
             : base(oldRDom)
        { }

        // new here feels wrong. Expect testing issues here
        public new ITypeParameter Copy()
        {
            return new RDomTypeParameter(this);
        }

        public bool SameIntent(ITypeParameter other)
        {
            return SameIntent(other, true);
        }

       public bool SameIntent(ITypeParameter other, bool includePublicAnnotations)
        {
            var otherItem = other as RDomTypeParameter;
            if (!base.SameIntent(otherItem, includePublicAnnotations)) return false;
            if (HasConstructorConstraint != other.HasConstructorConstraint) return false;
            if (HasReferenceConstraint != other.HasReferenceConstraint) return false;
            if (HasValueTypeConstraint != other.HasValueTypeConstraint) return false;
            if (Ordinal != other.Ordinal) return false;
            if (Variance != other.Variance) return false;
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
