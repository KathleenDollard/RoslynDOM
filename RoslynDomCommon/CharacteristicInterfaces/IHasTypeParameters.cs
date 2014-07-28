using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasTypeParameters : IDom
    {
        RDomList<ITypeParameter> TypeParameters { get; }
    }
}
