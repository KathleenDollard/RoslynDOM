namespace RoslynDom.Common
{
    public class SameIntent_IHasAccessModifier : ISameIntent<IHasAccessModifier>
    {
        public bool SameIntent(IHasAccessModifier one, IHasAccessModifier other, bool includePublicAnnotations)
        {
            if (one.AccessModifier != other.AccessModifier) { return false; }
            return true;
        }
    }
}
