using System;
using System.Collections.Concurrent;

namespace RoboCoP.Helpers
{
    public static class ConcurentDictionaryExtension
    {
        /// <summary>
        /// Adds the <paramref name="value"/> with the <paramref name="key"/> to the <paramref name="dictionary"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The same key already exists in the dictionary.</exception>
        public static void Add<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if (!dictionary.TryAdd(key, value))
                throw new ArgumentException("The element with the same key exists in the dictionary");
        }

    }
}