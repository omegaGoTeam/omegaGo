using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents a collection of game in a SGF file
    /// </summary>
    public class SgfCollection : IEnumerable<SgfGameTree>
    {
        /// <summary>
        /// Initializes SGF game collection
        /// </summary>
        /// <param name="gameTrees">Games to add to the collection</param>
        public SgfCollection( IEnumerable<SgfGameTree> gameTrees )
        {
            if ( gameTrees == null ) throw new ArgumentNullException( nameof( gameTrees ) );

            GameTrees = gameTrees;
        }

        /// <summary>
        /// List of games
        /// </summary>
        public IEnumerable<SgfGameTree> GameTrees { get; }

        /// <summary>
        /// Gets the generic collection's game trees enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<SgfGameTree> GetEnumerator() => GameTrees.GetEnumerator();

        /// <summary>
        /// Gets the non-generic collection's game trees enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}