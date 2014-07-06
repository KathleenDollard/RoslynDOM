using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class SameIntentBaseType : ISameIntent<IType>
    {
        private SameIntentCommon sameIntentCommon = new SameIntentCommon();
        public bool SameIntent(IType one, IType other, bool includePublicAnnotations)
        {
            if (!base.CheckSameIntent(other, includePublicAnnotations)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Fields, other.Fields)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Properties, other.Properties)) return false;
            if (!SameIntentHelpers.CheckSameIntentChildList(one.Methods, other.Methods)) return false;
            return true;
        }
    }
}
