namespace RoslynDom.Common
{

    public class SameIntent_IHasReturnType : ISameIntent<IHasReturnType>
    {
        public bool SameIntent(IHasReturnType one, IHasReturnType other, bool skipPublicAnnotations)
        {
            if (!one.ReturnType.SameIntent(other.ReturnType)) { return false; }
            return true;
        }
    }
}
