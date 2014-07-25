using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IHasTypeParameters : ISameIntent<IHasTypeParameters>
    {
        public bool SameIntent(IHasTypeParameters one, IHasTypeParameters other, bool skipPublicAnnotations)
        {
            if (!SameIntentHelpers.CheckSameIntentChildList(one.TypeParameters, other.TypeParameters, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}
