using System.Collections.Generic;

namespace RoslynDom.Common
{
  public class SameIntent_IHasImplementedInterfaces : ISameIntent<IHasImplementedInterfaces>
    {
        public bool SameIntent(IHasImplementedInterfaces one, IHasImplementedInterfaces other, bool skipPublicAnnotations)
        {
            if (!SameIntentHelpers.CheckSameIntentChildList(one.AllImplementedInterfaces, other.AllImplementedInterfaces, skipPublicAnnotations)) { return false; }
            return true;
        }
    }}
