using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface ITypeParameter : IMisc, IDom<ITypeParameter>, IReferencedType 
    {
        Variance Variance { get; }
        bool HasConstructorConstraint { get; }
        bool HasReferenceTypeConstraint { get; }
        bool HasValueTypeConstraint { get; }
        IEnumerable<IReferencedType > ConstraintTypes { get; }

        int Ordinal { get; }
    }
}
