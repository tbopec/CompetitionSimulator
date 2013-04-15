using System;
using System.Collections.Generic;

namespace RoboCoP.Helpers
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// Invoke <paramref name="action"/> for each element of <paramref name="enumerable"/>.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if(enumerable == null)
                throw new ArgumentNullException("enumerable");
            if(action == null)
                throw new ArgumentNullException("action");
            foreach(T t in enumerable)
                action(t);
        }
    }
}