using System;
using System.Collections.Generic;

namespace RoboCoP.Helpers
{
    public static class DictionaryExtension
    {
        /// <summary>
        /// Add a value <paramref name="val"/> to the <paramref name="dictionary"/>
        /// with the key <paramref name="key"/> and return this dictionary.
        /// </summary>
        public static IDictionary<TKey, TVal> Append<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, TKey key,
                                                                 TVal val)
        {
            if(dictionary == null)
                throw new ArgumentNullException("dictionary");
            dictionary.Add(key, val);
            return dictionary;
        }

        /// <summary>
        /// Get value from <paramref name="dictionary"/> by the <paramref name="key"/>. If such value do not exist then return <paramref name="defaultVal"/>.
        /// </summary>
        public static TVal GetOrDefault<TKey, TVal>(this IDictionary<TKey, TVal> dictionary, TKey key,
                                                    TVal defaultVal = default(TVal))
        {
            if(dictionary == null)
                throw new ArgumentNullException("dictionary");
            TVal val;
            if(!dictionary.TryGetValue(key, out val))
                val = defaultVal;
            return val;
        }
    }
}