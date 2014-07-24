using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IStemContainer : INestedContainer
    {
        IEnumerable<IUsingDirective> UsingDirectives { get; }
        RDomList<IStemMemberCommentWhite> StemMembersAll { get; }
        IEnumerable<IStemMember> StemMembers { get; }
        IEnumerable<INamespace> Namespaces { get; }
        IEnumerable<INamespace> AllChildNamespaces { get; }
        IEnumerable<INamespace> NonemptyNamespaces { get; }
    }
}