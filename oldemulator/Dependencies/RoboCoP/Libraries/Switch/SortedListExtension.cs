using System.Collections.Generic;
using System.Linq;

namespace Switch.Helpers
{
    public static class SortedListExtension
    {
        public static TKey GetKeyByValue<TKey, TVal>(this SortedList<TKey, TVal> sortedList, TVal x)
        {
            return sortedList.ElementAt(sortedList.IndexOfValue(x)).Key;
        }
    }
}