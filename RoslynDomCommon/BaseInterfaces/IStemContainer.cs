using System.Collections.Generic;

namespace RoslynDom.Common
{
    public interface IStemContainer : INestedContainer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Usings")]
        IEnumerable<IUsing> Usings { get; }
        IEnumerable<IStemMember> Members { get; }
        IEnumerable<INamespace> Namespaces { get; }
        IEnumerable<INamespace> AllChildNamespaces { get; }
        IEnumerable<INamespace> NonemptyNamespaces { get; }
    }
}