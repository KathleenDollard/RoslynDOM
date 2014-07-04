using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public interface IHasSameIntentMethod
    {
        bool SameIntent<T>(T other, bool includePublicAnnotations)
                where T : class;
        bool SameIntent<T>(T other)
                where T : class;
    }
}
