namespace RoslynDom.Common
{
    public class SameIntent_IUsing : ISameIntent<IUsing>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IStemMember> sameIntent_IStemMember = new SameIntent_IStemMember();

        public bool SameIntent(IUsing one, IUsing other, bool includePublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IStemMember.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
