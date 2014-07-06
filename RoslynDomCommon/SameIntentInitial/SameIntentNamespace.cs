using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class SameIntentNamespace : ISameIntent<INamespace>
    {
        private SameIntentStemContainer sameIntentStemContainer = new SameIntentStemContainer();
        public bool SameIntent(INamespace one, INamespace other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            // Base class checks classes, etc
            if (!sameIntentStemContainer.SameIntent(one,other, includePublicAnnotations)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.NonemptyNamespaces, other.NonemptyNamespaces)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.AllChildNamespaces, other.AllChildNamespaces)) return false;
            return true;
        }
    }
}