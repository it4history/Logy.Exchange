using System;
using System.Collections.Generic;

namespace Logy.Maps
{
    /// <summary>
    /// https://stackoverflow.com/questions/594518/is-there-a-lower-bound-function-on-a-sortedlistk-v
    /// The "no copy for Keys and Values" is the main diference with a SortedDictionary
    /// </summary>
    /// <returns></returns>
    public static class SortUtils
    {
        private static int BinarySearch<T>(IList<T> list, T value)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            var comp = Comparer<T>.Default;
            int lo = 0, hi = list.Count - 1;
            while (lo < hi)
            {
                int m = (hi + lo) / 2;  // this might overflow; be careful.
                if (comp.Compare(list[m], value) < 0) lo = m + 1;
                else hi = m - 1;
            }
            if (comp.Compare(list[lo], value) < 0) lo++;
            return lo;
        }

        public static int FindFirstIndexGreaterThanOrEqualTo<T, U>
            (this SortedList<T, U> sortedList, T key)
        {
            return BinarySearch(sortedList.Keys, key);
        }
    }
}