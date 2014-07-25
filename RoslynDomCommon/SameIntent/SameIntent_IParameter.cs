namespace RoslynDom.Common
{
    public class SameIntent_IParameter : ISameIntent<IParameter>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IHasAttributes> sameIntent_IHasAttributes = new SameIntent_IHasAttributes();

        public bool SameIntent(IParameter one, IParameter other, bool skipPublicAnnotations)
        {
            if (one.IsOut != other.IsOut) { return false; }
            if (one.IsRef != other.IsRef) { return false; }
            if (one.IsParamArray != other.IsParamArray) { return false; }
            if (one.IsOptional != other.IsOptional) { return false; }
            if (one.Ordinal != other.Ordinal) { return false; }
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IHasAttributes.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}
