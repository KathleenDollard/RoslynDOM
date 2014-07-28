using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface ISameIntent
    {  }

    public interface ISameIntent<T> :ISameIntent
    {
        bool SameIntent(T one, T other, bool skipPublicAnnotations);
    }
}
