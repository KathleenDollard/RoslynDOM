using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IType : ISameIntent<IType>
    {

        private ISameIntent<IStemMember> sameIntent_IStemMember = new SameIntent_IStemMember();
        private ISameIntent<ITypeMember> sameIntent_ITypeMember = new SameIntent_ITypeMember();

        public bool SameIntent(IType one, IType other, bool skipPublicAnnotations)
        {
            if (!sameIntent_IStemMember.SameIntent(one, other, skipPublicAnnotations)) { return false; }  // TESTCOVERAGE: Not testable
            if (!sameIntent_ITypeMember.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            return true;
        }
    }
}