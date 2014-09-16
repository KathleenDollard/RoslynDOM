using System.Collections.Generic;

namespace RoslynDom.Common
{
    public struct Foo
    {
        string test;
        public static Foo operator +(Foo x, Foo y)
        {
            return default(Foo);
        }
    }
    public interface ITypeMemberContainer : IDom
    {
        RDomCollection<ITypeMemberCommentWhite> MembersAll { get; }
        IEnumerable<ITypeMember> Members { get; }
        IEnumerable<IProperty> Properties { get; }
        IEnumerable<IMethod> Methods { get; }

    }
}
