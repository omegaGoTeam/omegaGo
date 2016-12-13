using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Extensions
{
    public static class ListExtensions
    {
        private static Random rng = new Random();

        /// <summary>
        /// Shuffles the specified list so that its elements are then in a random order.
        /// 
        /// Source: http://stackoverflow.com/a/1262619/1580088 
        /// </summary>
        /// <param name="list">The list to randomize.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            // 
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        /// <summary>
        /// Performs a specified ACTION for each descendant of this tree node, using a given FUNCTION
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
