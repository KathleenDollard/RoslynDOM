using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IDom : ISameIntent<IDom>
    {
        public bool SameIntent(IDom one, IDom other, bool skipPublicAnnotations)
        {
            if (one.GetType() != other.GetType()) { return false; }
            // Explicitly do not compare RawItems or OuterName
            var oneAsHasName = one as IHasName;
            if (oneAsHasName != null)
            {
                if (oneAsHasName.Name != ((IHasName)other).Name) { return false; }
            }
            if (!skipPublicAnnotations )
            {
                if(!one.PublicAnnotations.SameIntent(other.PublicAnnotations, skipPublicAnnotations)){ return false; }
            }
            return true;
        }
    }
}