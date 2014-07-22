using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class SameIntentHelpers
    {
        public static bool CheckSameIntentChildList<TChild>(
                    IEnumerable<TChild> thisList,
                    IEnumerable<TChild> otherList,
                    bool publicAnnotations)
             where TChild : class, IDom
        {
            if (thisList == null) return (otherList == null);
            if (otherList == null) return false;
            if (thisList.Count() != otherList.Count()) return false;
            if (thisList == null) return false; // can't happen, suppresse FxCop error
            foreach (var item in thisList)
            {
                var otherItem = otherList.Where(x => item.Matches(x)).FirstOrDefault();
                if (otherItem == null) return false;
                if (!item.SameIntent(otherItem, publicAnnotations)) return false;
            }
            return true;
        }

    }
}
