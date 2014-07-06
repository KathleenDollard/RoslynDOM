using System;
using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_ITypeMemberContainer : ISameIntent<ITypeMemberContainer>
    {

        public bool SameIntent(ITypeMemberContainer one, ITypeMemberContainer other, bool includePublicAnnotations)
        {
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Properties, other.Properties, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Methods, other.Methods, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Fields, other.Fields, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
