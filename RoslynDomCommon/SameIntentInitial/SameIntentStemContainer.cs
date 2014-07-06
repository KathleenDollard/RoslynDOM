using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class SameIntentStemContainer : ISameIntent<IStemContainer>
    {
        private SameIntentCommon sameIntentCommon = new SameIntentCommon();
        public bool SameIntent(IStemContainer one, IStemContainer other, bool includePublicAnnotations)
        {
            if (!sameIntentCommon.CheckSameIntent(one,other, includePublicAnnotations)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Classes, other.Classes)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Interfaces, other.Interfaces)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Structures, other.Structures)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Enums, other.Enums)) return false;
            return true;
        }
    }
}
