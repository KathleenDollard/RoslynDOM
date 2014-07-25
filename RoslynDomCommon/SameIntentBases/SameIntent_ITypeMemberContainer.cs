using System;
using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_ITypeMemberContainer : ISameIntent<ITypeMemberContainer>
    {

        public bool SameIntent(ITypeMemberContainer one, ITypeMemberContainer other, bool skipPublicAnnotations)
        {
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Properties, other.Properties, skipPublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Methods, other.Methods, skipPublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Fields, other.Fields, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}
