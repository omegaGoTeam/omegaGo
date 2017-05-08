using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents a tree branch in SGF file
    /// </summary>
    public class SgfGameTree
    {
        /// <summary>
        /// Creates a game tree
        /// </summary>
        /// <param name="sequence">Sequence of nodes</param>
        /// <param name="children">Child game trees</param>
        public SgfGameTree( SgfSequence sequence, IEnumerable<SgfGameTree> children )
        {
            if ( sequence == null ) throw new ArgumentNullException( nameof( sequence ) );
            if ( children == null ) throw new ArgumentNullException( nameof( children ) );

            Sequence = sequence;
            Children = new List<SgfGameTree>(children);
        }

        /// <summary>
        /// Sequence of SGF nodes
        /// </summary>
        public SgfSequence Sequence { get; }

        /// <summary>
        /// Child trees
        /// </summary>
        public ICollection<SgfGameTree> Children { get; }

        /// <summary>
        /// Gathers all game info properties from the tree
        /// </summary>
        /// <returns>Game info</returns>
        public SgfGameInfo GetGameInfo()
        {
            var searcher = new SgfGameInfoSearcher( this );
            return searcher.GetGameInfo();
        }

        /// <summary>
        /// Gets the value of the given SGF property if it's in the root node's SGF node sequence. Use this to get information about a game tree
        /// that is supplied in the root node.
        /// </summary>
        /// <typeparam name="T">Type of the property. Make sure this is correct.</typeparam>
        /// <param name="name">The name of the SGF property.</param>
        /// <returns></returns>
        public SgfProperty GetPropertyInSequence(string name)
        {
            foreach (var node in Sequence)
            {
                if (node[name] != null)
                {
                    return node[name];
                }
            }
            return null;
        }
    }
}
