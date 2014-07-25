using System;
using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IPropertyOrMethod : ISameIntent<IPropertyOrMethod>
    {
        private ISameIntent<ITypeMember> sameIntent_ITypeMember = new SameIntent_ITypeMember();
        private ISameIntent<ICanBeStatic > sameIntent_ICanBeStatic = new SameIntent_ICanBeStatic();
        private ISameIntent<IHasReturnType> sameIntent_IHasReturnType = new SameIntent_IHasReturnType();

        public bool SameIntent(IPropertyOrMethod one, IPropertyOrMethod other, bool skipPublicAnnotations)
        {
            if (one.IsAbstract != other.IsAbstract) { return false; }
            if (one.IsVirtual != other.IsVirtual) { return false; }
            if (one.IsOverride != other.IsOverride) { return false; }
            if (one.IsSealed != other.IsSealed) { return false; }
            if (!sameIntent_ITypeMember.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_ICanBeStatic.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IHasReturnType.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Parameters, other.Parameters, skipPublicAnnotations)) { return false; }
            return true;
        }
    }

}
