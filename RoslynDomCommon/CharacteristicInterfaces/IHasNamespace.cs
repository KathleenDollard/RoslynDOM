using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IHasNamespace
    {
        string Namespace { get; }
        string QualifiedName { get; }
    }
}
