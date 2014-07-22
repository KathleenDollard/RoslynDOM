using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasTypeParameters 
    {
        RDomList<ITypeParameter> TypeParameters { get; }
    }
}
