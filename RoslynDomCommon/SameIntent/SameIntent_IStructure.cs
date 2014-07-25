namespace RoslynDom.Common
{

    public class SameIntent_IStructure : ISameIntent<IStructure>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IType> sameIntent_IType = new SameIntent_IType();
        private ISameIntent<INestedContainer> sameIntent_INestedContainer = new SameIntent_INestedContainer();
        private ISameIntent<ITypeMemberContainer> sameIntent_ITypeMemberContainer = new SameIntent_ITypeMemberContainer();
        private ISameIntent<IHasTypeParameters> sameIntent_IHasTypeParameters = new SameIntent_IHasTypeParameters();
        private ISameIntent<IHasImplementedInterfaces> sameIntent_IHasImplementedInterfaces = new SameIntent_IHasImplementedInterfaces();

        public bool SameIntent(IStructure one, IStructure other, bool skipPublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IType.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_INestedContainer.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_ITypeMemberContainer.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IHasTypeParameters.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IHasImplementedInterfaces.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}