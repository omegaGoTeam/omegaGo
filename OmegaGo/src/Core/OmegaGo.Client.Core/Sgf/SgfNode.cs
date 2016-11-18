using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents a node in SGF
    /// </summary>
    internal class SgfNode : IEnumerable<SgfProperty>
    {
        /// <summary>
        /// Creates a SGF node
        /// </summary>
        /// <param name="properties">Properties contained in the node</param>
        public SgfNode( IEnumerable<SgfProperty> properties )
        {
            if ( properties == null ) throw new ArgumentNullException( nameof( properties ) );
            Properties = properties;
        }

        /// <summary>
        /// Node's properties
        /// </summary>
        public IEnumerable<SgfProperty> Properties { get; }

        /// <summary>
        /// Gets the generic node's properties enumerator
        /// </summary>
        /// <returns>Node's properties</returns>
        public IEnumerator<SgfProperty> GetEnumerator() => Properties.GetEnumerator();

        /// <summary>
        /// Gets the non-generic nonde's properties enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
