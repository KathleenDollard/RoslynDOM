using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IStemContainer : ISameIntent<IStemContainer>
    {
        public bool SameIntent(IStemContainer one, IStemContainer other, bool includePublicAnnotations)
        {
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Usings, other.Usings, includePublicAnnotations)) { return false; }
            // TODO: Take another look at this
            if (!SameIntentHelpers.CheckSameIntentChildList(one.StemMembers, other.StemMembers, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.AllChildNamespaces, other.AllChildNamespaces, includePublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.NonemptyNamespaces, other.NonemptyNamespaces, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}