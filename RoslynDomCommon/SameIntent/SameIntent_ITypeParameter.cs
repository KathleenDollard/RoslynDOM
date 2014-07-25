using System.Collections.Generic;

namespace RoslynDom.Common
{


    public class SameIntent_ITypeParameter : ISameIntent<ITypeParameter>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IReferencedType> sameIntent_IReferencedType = new SameIntent_IReferencedType();

        public bool SameIntent(ITypeParameter one, ITypeParameter other, bool skipPublicAnnotations)
        {
            if (one.Variance != other.Variance) { return false; }
            if (one.HasConstructorConstraint != other.HasConstructorConstraint) { return false; }
            if (one.HasReferenceTypeConstraint != other.HasReferenceTypeConstraint) { return false; }
            if (one.HasValueTypeConstraint != other.HasValueTypeConstraint) { return false; }
            if (one.Ordinal != other.Ordinal) { return false; }
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IReferencedType.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.ConstraintTypes, other.ConstraintTypes, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}
