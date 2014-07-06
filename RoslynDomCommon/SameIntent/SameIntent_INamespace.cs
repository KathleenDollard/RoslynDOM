namespace RoslynDom.Common
{

    public class SameIntent_INamespace : ISameIntent<INamespace>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IStemMember> sameIntent_IStemMember = new SameIntent_IStemMember();
        private ISameIntent<IStemContainer> sameIntent_IStemContainer = new SameIntent_IStemContainer();

        public bool SameIntent(INamespace one, INamespace other, bool includePublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IStemMember.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IStemContainer.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}