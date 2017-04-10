using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Extensions
{
    public static class TreeExtensions
    {
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
            foreach (var child in getChildren(tree))
            {
                child.ForAllDescendants(getChildren, actionPerNode);
            }
        }
    }
}
