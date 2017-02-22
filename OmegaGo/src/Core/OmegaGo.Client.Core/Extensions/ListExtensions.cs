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
                int k = Randomizer.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }  
    }
}
