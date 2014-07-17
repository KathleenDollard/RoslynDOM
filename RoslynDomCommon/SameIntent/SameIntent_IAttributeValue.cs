namespace RoslynDom.Common
{
    public class SameIntent_IAttributeValue : ISameIntent<IAttributeValue>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        public bool SameIntent(IAttributeValue one, IAttributeValue other, bool includePublicAnnotations)
        {
            if (one.Value == null && other.Value == null) return true;
            if (one.Value == null && other.Value != null) return false;
            if (one.Value != null && other.Value == null) return false;
            if (!one.Value.Equals(other.Value)) { return false; }
            if (one.ValueType != other.ValueType) { return false; }
            if (!sameIntent_IDom.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
