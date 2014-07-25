namespace RoslynDom.Common
{

    public class SameIntent_IInvalidTypeMember : ISameIntent<IInvalidTypeMember>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<ITypeMember> sameIntent_ITypeMember = new SameIntent_ITypeMember();

        public bool SameIntent(IInvalidTypeMember one, IInvalidTypeMember other, bool skipPublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            if (!sameIntent_ITypeMember.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}
