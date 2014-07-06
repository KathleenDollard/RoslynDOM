using System;

namespace RoslynDom.Common
{
    public class SameIntent_ITypeMember : ISameIntent<ITypeMember>
    {

        private ISameIntent<IHasAttributes> sameIntent_IHasAttributes = new SameIntent_IHasAttributes();
        private ISameIntent<IHasAccessModifier> sameIntent_IHasAccessModifier = new SameIntent_IHasAccessModifier();
        private ISameIntent<IMember> sameIntent_IMember = new SameIntent_IMember();

        public bool SameIntent(ITypeMember one, ITypeMember other, bool includePublicAnnotations)
        {
            if (one.MemberType != other.MemberType) { return false; }
            if (!sameIntent_IHasAttributes.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IHasAccessModifier.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IMember.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
