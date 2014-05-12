using System.Collections.Generic;

namespace RoslynK
{
    public interface IStemContainer : INestedContainer
    {
        IEnumerable<IUsing> Usings { get; }
        IEnumerable<IStemMember> Members { get; }
        IEnumerable<INamespace> Namespaces { get; }
    }
}