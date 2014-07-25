namespace RoslynDom.Common
{
    public class SameIntent_ICanBeStatic : ISameIntent<ICanBeStatic>
    {
        public bool SameIntent(ICanBeStatic one, ICanBeStatic other, bool skipPublicAnnotations)
        {
            if (one.IsStatic != other.IsStatic) { return false; }
            return true;
        }
    }
}
