namespace RoslynDom.Common
{
    public class SameIntent_IEnumValue : ISameIntent<IEnumValue>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();

        public bool SameIntent(IEnumValue one, IEnumValue other, bool skipPublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (one.Expression != null)
            { if (!one.Expression.SameIntent(other.Expression)) { return false; } }
            return true;
        }
    }
}