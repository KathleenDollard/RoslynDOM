using System;
using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IAttribute : ISameIntent<IAttribute>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        public bool SameIntent(IAttribute one, IAttribute other, bool includePublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.AttributeValues , other.AttributeValues, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
