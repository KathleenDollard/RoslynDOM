namespace RoslynDom.Common
{
    public class SameIntent_IUsingDirective : ISameIntent<IUsingDirective>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IStemMember> sameIntent_IStemMember = new SameIntent_IStemMember();

        public bool SameIntent(IUsingDirective one, IUsingDirective other, bool includePublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IStemMember.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
