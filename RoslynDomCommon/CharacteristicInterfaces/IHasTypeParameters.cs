using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IHasTypeParameters 
    {
        IEnumerable<ITypeParameter> TypeParameters { get; }
    }
}
