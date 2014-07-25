using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.Common
{
    public class SameIntent_IStemContainer : ISameIntent<IStemContainer>
    {
        public bool SameIntent(IStemContainer one, IStemContainer other, bool skipPublicAnnotations)
        {
            if (!SameIntentHelpers.CheckSameIntentChildList(one.UsingDirectives, other.UsingDirectives, skipPublicAnnotations)) { return false; }
            // Skip whitespace , but not comments on comparison.
            var oneMembers = one.StemMembersAll.Where(x => !(x is IVerticalWhitespace));
            var otherMembers = other.StemMembersAll.Where(x => !(x is IVerticalWhitespace));
            if (!SameIntentHelpers.CheckSameIntentChildList(oneMembers, otherMembers, skipPublicAnnotations)) { return false; }
            return true;

        }
    }
}