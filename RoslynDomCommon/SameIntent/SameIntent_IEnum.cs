namespace RoslynDom.Common
{
    public class SameIntent_IEnum : ISameIntent<IEnum>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IType> sameIntent_IType = new SameIntent_IType();

        public bool SameIntent(IEnum one, IEnum other, bool skipPublicAnnotations)
        {
            if (!one.UnderlyingType.SameIntent(other.UnderlyingType)) { return false; }
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IType.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Values, other.Values, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}