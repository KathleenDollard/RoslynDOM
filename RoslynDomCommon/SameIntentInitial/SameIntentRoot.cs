using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class SameIntentRoot : ISameIntent<IRoot>
    {
        private SameIntentStemContainer sameIntentStemContainer = new SameIntentStemContainer();
        public bool SameIntent(IRoot one, IRoot other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            // Base class checks classes, etc
            if (!sameIntentStemContainer.SameIntent(one,other, includePublicAnnotations)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.NonemptyNamespaces, other.NonemptyNamespaces)) return false;
            return true;
        }
    }
}
