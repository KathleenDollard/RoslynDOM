namespace RoslynDom.Common
{
    public class SameIntent_IField : ISameIntent<IField>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<ITypeMember> sameIntent_ITypeMember = new SameIntent_ITypeMember();
        private ISameIntent<ICanBeStatic> sameIntent_ICanBeStatic = new SameIntent_ICanBeStatic();
        private ISameIntent<IHasReturnType > sameIntent_IHasReturnType = new SameIntent_IHasReturnType();

        public bool SameIntent(IField one, IField other, bool includePublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_ITypeMember.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_ICanBeStatic.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_IHasReturnType.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}