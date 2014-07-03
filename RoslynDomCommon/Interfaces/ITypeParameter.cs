using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface ITypeParameter : IDom<ITypeParameter>
    {
        Variance Variance { get; }
        bool HasConstructorConstraint { get; }
        bool HasReferenceConstraint { get; }
        bool HasValueTypeConstraint { get; }
        IEnumerable<IReferencedType > ConstraintTypes { get; }

        int Ordinal { get; }
    }
}
