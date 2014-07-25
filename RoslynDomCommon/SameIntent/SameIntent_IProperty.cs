namespace RoslynDom.Common
{

    public class SameIntent_IProperty : ISameIntent<IProperty>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IPropertyOrMethod> sameIntent_IPropertyOrMethod = new SameIntent_IPropertyOrMethod();

        public bool SameIntent(IProperty one, IProperty other, bool skipPublicAnnotations)
        {
            if (one.CanGet != other.CanGet) { return false; }
            if (one.CanSet != other.CanSet) { return false; }
            if (!one.PropertyType.SameIntent( other.PropertyType)) { return false; }
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IPropertyOrMethod.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}