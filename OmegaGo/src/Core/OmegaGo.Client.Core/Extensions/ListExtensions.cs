using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Helpers;

namespace OmegaGo.Core.Extensions
{
    /// <summary>
    /// Extensions for list-based types
    /// </summary>
    internal static class ListExtensions
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Shuffles the specified list in-place so that its elements are then in a random order.
        /// 
        /// Source: http://stackoverflow.com/a/1262619/1580088 
        /// </summary>
        /// <param name="list">The list to randomize.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Adds item into collection if it is not null
        /// </summary>
        /// <typeparam name="T">Item type</typeparam>
        /// <param name="list">List</param>
        /// <param name="addedItem">Added item</param>
        public static void AddIfNotNull<T>(this IList<T> list, T addedItem) where T : class
        {
            if (addedItem != null)
            {
                list.Add(addedItem);
            }
        }

        /// <summary>
        /// Removes all items matching a given predicate
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <param name="list">List</param>
        /// <param name="predicate">Filter</param>
        public static void RemoveAll<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}
