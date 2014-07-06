namespace RoslynDom.Common
{

    public class SameIntent_IHasNamespace : ISameIntent<IHasNamespace>
    {
        public bool SameIntent(IHasNamespace one, IHasNamespace other, bool includePublicAnnotations)
        {
            if (one.Namespace != other.Namespace) { return false; }
            if (one.QualifiedName != other.QualifiedName) { return false; }
            return true;
        }
    }
}
