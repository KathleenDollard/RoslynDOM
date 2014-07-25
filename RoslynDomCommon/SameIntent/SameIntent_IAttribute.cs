using System;
using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IAttribute : ISameIntent<IAttribute>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        public bool SameIntent(IAttribute one, IAttribute other, bool skipPublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.AttributeValues , other.AttributeValues, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}
