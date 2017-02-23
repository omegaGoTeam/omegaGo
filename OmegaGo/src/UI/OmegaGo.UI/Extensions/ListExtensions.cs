using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Extensions
{
    internal static class ListExtensions
    {
        private static readonly Random Randomizer = new Random();

        /// <summary>
        /// Returns random item from a list
        /// </summary>
        /// <typeparam name="T">Type of the list items</typeparam>
        /// <param name="list">List</param>
        /// <returns>Random item</returns>
        public static T GetRandom<T>(this IList<T> list)
        {
            int index = Randomizer.Next(list.Count);
            return list[index];
        }
    }
}
