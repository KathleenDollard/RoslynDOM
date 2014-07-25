using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IReferencedType : ISameIntent<IReferencedType>
    {
        private ISameIntent<IDom> sameIntent_IDom = new SameIntent_IDom();
        private ISameIntent<IHasNamespace> sameIntent_IHasNamespace = new SameIntent_IHasNamespace();

        public bool SameIntent(IReferencedType one, IReferencedType other, bool skipPublicAnnotations)
        {
            if (!sameIntent_IDom.SameIntent(one, other, skipPublicAnnotations)) { return false; }
            // Explicilty not calling
            //if (!sameIntent_IHasNamespace.SameIntent(one, other, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}
