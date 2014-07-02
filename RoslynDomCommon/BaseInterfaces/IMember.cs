using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification ="Used to group Stem members and Type members in RDomFactory only. May reconsider")]
    public interface IMember : IDom
    {
    }
}
