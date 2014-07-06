using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IType : ISameIntent<IType>
    {

        private ISameIntent<IStemMember> sameIntent_IStemMember = new SameIntent_IStemMember();
        private ISameIntent<ITypeMember> sameIntent_ITypeMember = new SameIntent_ITypeMember();

        public bool SameIntent(IType one, IType other, bool includePublicAnnotations)
        {
            if (!sameIntent_IStemMember.SameIntent(one, other, includePublicAnnotations)) { return false; }
            if (!sameIntent_ITypeMember.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}