using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IRoot : ISameIntent<IRoot>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();

        public bool SameIntent(IRoot one, IRoot other, bool includePublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.RootClasses, other.RootClasses, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.RootInterfaces, other.RootInterfaces, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.RootEnums, other.RootEnums, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.RootStructures, other.RootStructures, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
