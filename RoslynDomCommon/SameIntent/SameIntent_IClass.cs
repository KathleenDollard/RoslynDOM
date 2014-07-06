using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IClass : ISameIntent<IClass>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IType> sameIntent_IType = new SameIntent_IType();
        private ISameIntent<INestedContainer> sameIntent_INestedContainer = new SameIntent_INestedContainer();
        private ISameIntent<ITypeMemberContainer> sameIntent_ITypeMemberContainer = new SameIntent_ITypeMemberContainer();
        private ISameIntent<IHasTypeParameters> sameIntent_IHasTypeParameters = new SameIntent_IHasTypeParameters();
        private ISameIntent<IHasImplementedInterfaces> sameIntent_IHasImplementedInterfaces = new SameIntent_IHasImplementedInterfaces();
        private ISameIntent<ICanBeStatic> sameIntent_ICanBeStatic = new SameIntent_ICanBeStatic();

        public bool SameIntent(IClass one, IClass other, bool includePublicAnnotations)
        {
            if (!one.BaseType.SameIntent(other.BaseType)) { return false; }
            if (one.IsAbstract != other.IsAbstract) { return false; }
            if (one.IsSealed != other.IsSealed) { return false; }
            if (!sameIntent_IDom.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IType.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_INestedContainer.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_ITypeMemberContainer.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IHasTypeParameters.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IHasImplementedInterfaces.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_ICanBeStatic.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}