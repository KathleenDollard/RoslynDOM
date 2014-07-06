namespace RoslynDom.Common
{
    public class SameIntent_IStemMember : ISameIntent<IStemMember>
    {
      public bool SameIntent(IStemMember one, IStemMember other, bool includePublicAnnotations)
        {
            if (one.StemMemberType != other.StemMemberType) { return false; }
            return true;
        }
    }
}
