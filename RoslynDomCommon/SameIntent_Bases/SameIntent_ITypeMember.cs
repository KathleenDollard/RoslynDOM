using System;

namespace RoslynDom.Common
{
    public class SameIntent_ITypeMember : ISameIntent<ITypeMember>
    {

        private ISameIntent<IHasAttributes> sameIntent_IHasAttributes = new SameIntent_IHasAttributes();
        private ISameIntent<IHasAccessModifier> sameIntent_IHasAccessModifier = new SameIntent_IHasAccessModifier();
        private ISameIntent<IMember> sameIntent_IMember = new SameIntent_IMember();

        public bool SameIntent(ITypeMember one, ITypeMember other, bool skipPublicAnnotations)
        {
            if (!sameIntent_IHasAttributes.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IHasAccessModifier.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            // Returning the value is a trick to exclude from code covereage
            return (sameIntent_IMember.SameIntent(one, other, skipPublicAnnotations));
        }
    }
}
