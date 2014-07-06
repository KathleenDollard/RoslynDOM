using System.Collections.Generic;

namespace RoslynDom.Common
{

    public class SameIntent_INestedContainer : ISameIntent<INestedContainer>
    {
        public bool SameIntent(INestedContainer one, INestedContainer other, bool includePublicAnnotations)
        {
            // TODO: FIx the following
            //if (!SameIntentHelpers.CheckSameIntentChildList(one.Types, other.Types, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Classes, other.Classes, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Structures, other.Structures, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Interfaces, other.Interfaces, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Enums, other.Enums, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}