using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IStemContainer : INestedContainer
    {
        IEnumerable<IUsing> Usings { get; }
        IEnumerable<IStemMember> Members { get; }
        IEnumerable<INamespace> Namespaces { get; }
        IEnumerable<INamespace> AllChildNamespaces { get; }
        IEnumerable<INamespace> NonEmptyNamespaces { get; }
    }
}