namespace RoslynDom.Common
{
    public class SameIntent_IInterface : ISameIntent<IInterface>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IType> sameIntent_IType = new SameIntent_IType();
        private ISameIntent<ITypeMemberContainer> sameIntent_ITypeMemberContainer = new SameIntent_ITypeMemberContainer();
        private ISameIntent<IHasTypeParameters> sameIntent_IHasTypeParameters = new SameIntent_IHasTypeParameters();
        private ISameIntent<IHasImplementedInterfaces> sameIntent_IHasImplementedInterfaces = new SameIntent_IHasImplementedInterfaces();

        public bool SameIntent(IInterface one, IInterface other, bool skipPublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IType.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_ITypeMemberContainer.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IHasTypeParameters.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IHasImplementedInterfaces.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}