using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface ISameIntent<T>
    {
        bool SameIntent(T one, T other, bool includePublicAnnotations);
    }
}
