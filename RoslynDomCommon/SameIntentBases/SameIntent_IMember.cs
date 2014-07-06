namespace RoslynDom.Common
{
    public class SameIntent_IMember : ISameIntent<IMember>
    {

        public bool SameIntent(IMember one, IMember other, bool includePublicAnnotations)
        {
            return true;
        }
    }
}
