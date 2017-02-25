using System.Collections.Generic;

namespace OmegaGo.UI.Extensions
{
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Returns a dictionary item or default value
        /// </summary>
        /// <typeparam name="TKey">Key</typeparam>
        /// <typeparam name="TValue">Value</typeparam>
        /// <param name="dictionary">Dictionary</param>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        internal static TValue ItemOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value = default(TValue);
            dictionary.TryGetValue(key, out value);
            return value;
        }
    }
}
