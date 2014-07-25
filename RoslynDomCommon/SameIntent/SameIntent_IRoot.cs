using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IRoot : ISameIntent<IRoot>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IStemContainer> sameIntent_IStemContainer = new SameIntent_IStemContainer();

        public bool SameIntent(IRoot one, IRoot other, bool skipPublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IStemContainer.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            //if (!SameIntentHelpers.CheckSameIntentChildList(one.RootClasses, other.RootClasses, skipPublicAnnotations)) { return false; }
            //if (!SameIntentHelpers.CheckSameIntentChildList(one.RootInterfaces, other.RootInterfaces, skipPublicAnnotations)) { return false; }
            //if (!SameIntentHelpers.CheckSameIntentChildList(one.RootEnums, other.RootEnums, skipPublicAnnotations)) { return false; }
            //if (!SameIntentHelpers.CheckSameIntentChildList(one.RootStructures, other.RootStructures, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}
