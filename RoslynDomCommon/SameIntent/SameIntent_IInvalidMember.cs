namespace RoslynDom.Common
{

    public class SameIntent_IInvalidTypeMember : ISameIntent<IInvalidTypeMember>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<ITypeMember> sameIntent_ITypeMember = new SameIntent_ITypeMember();

        public bool SameIntent(IInvalidTypeMember one, IInvalidTypeMember other, bool includePublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_ITypeMember.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
