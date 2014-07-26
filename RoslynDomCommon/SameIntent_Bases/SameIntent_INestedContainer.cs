using System.Collections.Generic;

namespace RoslynDom.Common
{

    public class SameIntent_INestedContainer : ISameIntent<INestedContainer>
    {
        public bool SameIntent(INestedContainer one, INestedContainer other, bool skipPublicAnnotations)
        {
            // TODO: FIx the following
            //if (!SameIntentHelpers.CheckSameIntentChildList(one.Types, other.Types, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Classes, other.Classes, skipPublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Structures, other.Structures, skipPublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Interfaces, other.Interfaces, skipPublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Enums, other.Enums, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}