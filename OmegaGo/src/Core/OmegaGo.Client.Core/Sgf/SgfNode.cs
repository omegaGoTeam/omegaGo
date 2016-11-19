using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents a node in SGF
    /// </summary>
    public class SgfNode : IEnumerable<SgfProperty>
    {
        /// <summary>
        /// Creates a SGF node
        /// </summary>
        /// <param name="properties">Properties contained in the node</param>
        public SgfNode( IEnumerable<SgfProperty> properties )
        {
            if ( properties == null ) throw new ArgumentNullException( nameof( properties ) );
            Properties = new ReadOnlyDictionary<string, SgfProperty>( properties.ToDictionary( p => p.Identifier, p => p ) );
        }

        /// <summary>
        /// Gets a property by identifier, null if not defined in node
        /// </summary>
        /// <param name="identifier">Identifier of the property</param>
        /// <returns>Property</returns>
        public SgfProperty this[ string identifier ]
        {
            get
            {
                SgfProperty property = null;
                return Properties.TryGetValue( identifier, out property ) ? property : null;
            }
        }

        /// <summary>
        /// Node's properties
        /// </summary>
        public IReadOnlyDictionary<string, SgfProperty> Properties { get; }

        /// <summary>
        /// Gets the generic node's properties enumerator
        /// </summary>
        /// <returns>Node's properties</returns>
        public IEnumerator<SgfProperty> GetEnumerator() => Properties.Values.GetEnumerator();

        /// <summary>
        /// Gets the non-generic nonde's properties enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
