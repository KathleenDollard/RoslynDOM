using System.Collections.Generic;

namespace RoslynDom.Common
{
  public class SameIntent_IHasImplementedInterfaces : ISameIntent<IHasImplementedInterfaces>
    {
        public bool SameIntent(IHasImplementedInterfaces one, IHasImplementedInterfaces other, bool includePublicAnnotations)
        {
            if (!SameIntentHelpers.CheckSameIntentChildList(one.AllImplementedInterfaces, other.AllImplementedInterfaces, includePublicAnnotations)) { return false; }
            return true;
        }
    }}
