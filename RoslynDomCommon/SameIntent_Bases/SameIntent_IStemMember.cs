namespace RoslynDom.Common
{
    public class SameIntent_IStemMember : ISameIntent<IStemMember>
    {
      public bool SameIntent(IStemMember one, IStemMember other, bool skipPublicAnnotations)
        {
            Guardian.Assert.IsTrue(one.StemMemberKind == other.StemMemberKind);
            return true;
        }
    }
}
