namespace RoslynDom.Common
{

    public class SameIntent_INamespace : ISameIntent<INamespace>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IStemMember> sameIntent_IStemMember = new SameIntent_IStemMember();
        private ISameIntent<IStemContainer> sameIntent_IStemContainer = new SameIntent_IStemContainer();

        public bool SameIntent(INamespace one, INamespace other, bool skipPublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_IStemMember.SameIntent(one, other, skipPublicAnnotations)) { return false; }  // TESTCOVERAGE: Not testable
            if (!sameIntent_IStemContainer.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}