using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Searches the game info in a given SGF game tree
    /// </summary>
    public class SgfGameInfoSearcher
    {
        private readonly SgfGameTree _gameTree;        
        private readonly Dictionary<string, SgfProperty> _gameInfoProperties = new Dictionary<string, SgfProperty>();

        public SgfGameInfoSearcher( SgfGameTree gameTree )
        {
            _gameTree = gameTree;
        }

        /// <summary>
        /// Gathers all game info properties from the game tree
        /// </summary>
        /// <returns>Game info</returns>
        public SgfGameInfo GetGameInfo()
        {            
            SearchGameTree( _gameTree );
            return new SgfGameInfo( _gameInfoProperties.Values.ToArray() );            
        }

        /// <summary>
        /// Searches a SGF game tree for game-info properties
        /// </summary>
        /// <param name="gameTree">SGF game tree</param>
        private void SearchGameTree( SgfGameTree gameTree )
        {
            foreach( var child in gameTree.Children )
            {
                SearchGameTree(child);
            }
            //gather game-info properties from this node
            foreach( var node in gameTree.Sequence )
            {
                foreach( var property in node )
                {
                    var knownProperty = SgfKnownProperties.Get( property.Identifier );
                    if ( knownProperty != null && knownProperty.Type == SgfPropertyType.GameInfo )
                    {
                        _gameInfoProperties[ property.Identifier ] = property;
                    }
                }
            }
        }        
    }
}
