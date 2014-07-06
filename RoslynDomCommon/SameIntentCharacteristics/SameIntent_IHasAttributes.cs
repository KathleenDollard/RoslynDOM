using System.Collections.Generic;

namespace RoslynDom.Common
{
     public class SameIntent_IHasAttributes : ISameIntent<IHasAttributes>
    {
        public bool SameIntent(IHasAttributes one, IHasAttributes other, bool includePublicAnnotations)
        {
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Attributes, other.Attributes, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
