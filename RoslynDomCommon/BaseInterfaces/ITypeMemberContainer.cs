using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface ITypeMemberContainer 
    {
        RDomList<ITypeMember> MembersAll { get; }
        IEnumerable<ITypeMember> Members { get; }
        IEnumerable<IProperty> Properties { get; }
        IEnumerable<IMethod> Methods { get; }
        IEnumerable<IField> Fields { get; }

    }
}
