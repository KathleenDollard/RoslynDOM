using System.Collections.Generic;

namespace RoslynDom.Common
{
    public class SameIntent_IDom : ISameIntent<IDom>
    {
        public bool SameIntent(IDom one, IDom other, bool includePublicAnnotations)
        {
            // Explicitly do not compare RawItems or OuterName
            if (one.Name != other.Name) { return false; }
            if (!one.PublicAnnotations.SameIntent( other.PublicAnnotations, includePublicAnnotations)) { return false; }
            return true;
        }
    }
}