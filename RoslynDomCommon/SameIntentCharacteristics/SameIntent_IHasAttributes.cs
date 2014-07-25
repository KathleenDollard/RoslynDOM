using System.Collections.Generic;

namespace RoslynDom.Common
{
     public class SameIntent_IHasAttributes : ISameIntent<IHasAttributes>
    {
        public bool SameIntent(IHasAttributes one, IHasAttributes other, bool skipPublicAnnotations)
        {
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Attributes.Attributes, other.Attributes.Attributes, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}
