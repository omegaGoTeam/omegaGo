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
    internal class SgfGameTree
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
            Children = children;
        }

        /// <summary>
        /// Sequence of SGF nodes
        /// </summary>
        public SgfSequence Sequence { get; }

        /// <summary>
        /// Child trees
        /// </summary>
        public IEnumerable<SgfGameTree> Children { get; }
    }
}
