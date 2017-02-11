using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Helpers;

namespace OmegaGo.Core.Extensions
{
    //TODO: This class should be internal, after Tsumego is moved into core
    /// <summary>
    /// Extensions for list-based types
    /// </summary>
    public static class ListExtensions
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

        /// <summary>
        /// Performs a specified action for each descendant of this tree node, using a given function
        /// to determine the children of each node. The action is performed using pre-order traversal.
        /// </summary>
        /// <param name="tree">The tree to walk through.</param>
        /// <param name="getChildren">This function returns all children of a node in the tree.</param>
        /// <param name="actionPerNode">The action is performed for each node in the tree.</param>
        public static void ForAllDescendants<T>(this T tree,
            Func<T, IEnumerable<T>> getChildren,
            Action<T> actionPerNode)
        {
            actionPerNode(tree);
            foreach(var child in getChildren(tree))
            {
                child.ForAllDescendants(getChildren, actionPerNode);
            }
        }
    }
}
