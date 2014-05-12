using System.Collections.Generic;

namespace RoslynK
{
    public interface ITypeMemberContainer 
    {
        IEnumerable<ITypeMember> Members { get; }
        IEnumerable<IProperty> Properties { get; }
        IEnumerable<IMethod> Methods { get; }
        IEnumerable<IField> Fields { get; }

    }
}
