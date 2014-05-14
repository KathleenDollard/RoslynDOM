using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
    public static class GeneralUtilities
    {
        /// <summary>
        /// Returns all unique combos of items in the list, along with the empty set
        /// </summary>
        /// <typeparam name="T">Type of the list passed, and returned</typeparam>
        /// <param name="initialList">The list whose combinitios will be returned</param>
        /// <returns>
        /// A list of lists, where each nested list is a unique combination of items in the initial list
        /// </returns>
        /// <remarks>
        /// From Benson Joeris suggesting the algorithm I coded here:<br/><br/>
        /// It sounds like you want to enumerate subsets of {1,...,N}. There are 2^N of these, 
        /// and they can fairly easily be encoded as the integers 1,...,(2^N)-1, by thinking 
        /// about the numbers in binary. If the least significant bit is 1, then string 1 is 
        /// included, and if it is 0, string 1 is not included. The next least significant bit 
        /// encodes whether string 2 is included, and so on. So, with a little bit manipulation, 
        /// you can decode an integer into the string arrays you are looking for, and then just 
        /// have to loop over 1,...,(2^N)-1 (or 0,...,(2^N)-1, if you want to include the empty 
        /// set of strings)
        /// </remarks>
        public static IEnumerable<IEnumerable<T>> GetAllCombos<T>(this List<T> initialList)
        {
            var ret = new List<List<T>>();
            if (initialList == null) { return ret; }
            var listCount = initialList.Count();
            // The final number of sets will be 2^N (or 2^N - 1 if skipping empty set)
            int setCount = 1 << listCount;

            // Start at 1 if you do not want the empty set
            for (int mask = 0; mask < setCount; mask++)
            {
                var nestedList = new List<T>();
                for (int j = 0; j < listCount; j++)
                {
                    // For large list in high perf code, precalculate bitmaps
                    // Each position in the initial list maps to a bit here
                    var pos = Convert.ToInt32(Math.Pow(2, j));
                    if ((mask & pos) == pos) { nestedList.Add(initialList[j]); }
                }
                ret.Add(nestedList);
            }
            return ret;
        }
    }
}
