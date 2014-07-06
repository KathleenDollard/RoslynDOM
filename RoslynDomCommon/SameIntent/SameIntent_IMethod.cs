namespace RoslynDom.Common
{

    public class SameIntent_IMethod : ISameIntent<IMethod>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IPropertyOrMethod> sameIntent_IPropertyOrMethod = new SameIntent_IPropertyOrMethod();
        private ISameIntent<IHasTypeParameters> sameIntent_IIHasTypeParameters = new SameIntent_IHasTypeParameters();

        public bool SameIntent(IMethod one, IMethod other, bool includePublicAnnotations)
        {
            if (one.IsExtensionMethod != other.IsExtensionMethod) { return false; }
            if (!sameIntent_IDom.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IPropertyOrMethod.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IIHasTypeParameters.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
