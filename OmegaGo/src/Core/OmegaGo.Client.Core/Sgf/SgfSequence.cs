using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// SGF sequence
    /// </summary>
    public class SgfSequence : IEnumerable<SgfNode>
    {
        /// <summary>
        /// Creates SGF sequence
        /// </summary>
        /// <param name="nodes">Nodes in the sequence (at least one node required)</param>
        public SgfSequence( IEnumerable<SgfNode> nodes )
        {
            if ( nodes == null ) throw new ArgumentNullException( nameof( nodes ) );
            var nodeArray = nodes.ToArray();
            if ( !nodeArray.Any() ) throw new ArgumentOutOfRangeException( nameof( nodes ), "There must be at least one node in the sequence" );
            Nodes = nodeArray;
        }

        /// <summary>
        /// Nodes in the SGF sequence
        /// </summary>
        public IReadOnlyList<SgfNode> Nodes { get; }

        /// <summary>
        /// Gets the generic sequence's nodes enumerator
        /// </summary>
        /// <returns>Generic enumerator</returns>
        public IEnumerator<SgfNode> GetEnumerator() => Nodes.GetEnumerator();

        /// <summary>
        /// Gets the non-generic sequence's nodes enumerator
        /// </summary>
        /// <returns>Non-generic enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
