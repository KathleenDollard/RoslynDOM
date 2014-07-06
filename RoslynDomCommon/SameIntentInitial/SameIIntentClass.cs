using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class SameIntentClass : ISameIntent<IClass>
    {
        private SameIntentStemContainer sameIntentStemContainer = new SameIntentStemContainer();
        public bool SameIntent(IClass one, IClass other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            if (!sameIntentStemContainer.SameIntent(one,other, includePublicAnnotations)) return false;
            if (one.IsAbstract != other.IsAbstract) return false;
            if (one.IsSealed != other.IsSealed) return false;
            if (one.IsStatic != other.IsStatic) return false;
            if (!one.BaseType.SameIntent(other.BaseType)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Classes, other.Classes)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Structures, other.Structures)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Interfaces, other.Interfaces)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Enums, other.Enums)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.TypeParameters, other.TypeParameters)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.AllImplementedInterfaces, other.AllImplementedInterfaces)) return false;
            return true;
        }
    }
}