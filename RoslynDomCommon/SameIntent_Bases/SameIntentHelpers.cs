using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public class SameIntentHelpers
    {
        // until move to C# 6 - I want to support name of as soon as possible
        private static string nameof<T>(T value) { return ""; }

        public static bool CheckSameIntentChildList<TChild>(
                    IEnumerable<TChild> thisList,
                    IEnumerable<TChild> otherList,
                    bool publicAnnotations)
             where TChild : class, IDom
        {
            Guardian.Assert.IsNotNull(thisList, nameof(thisList));
            Guardian.Assert.IsNotNull(otherList, nameof(otherList));
            if (thisList.Count() != otherList.Count()) return false;
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
