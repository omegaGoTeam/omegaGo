using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.GameState
{
    /// <summary>
    /// Serializes and deserializes the game tree
    /// </summary>
    public interface IGameTreeSerializer
    {
        /// <summary>
        /// Serializes the game tree to string
        /// </summary>
        /// <param name="tree">Game tree</param>
        /// <returns>Serialized tree</returns>
        string Serialize( GameTree tree );

        /// <summary>
        /// Deserializes the game tree from string
        /// </summary>
        /// <param name="serializedTree">Serialized tree</param>
        /// <returns>Game tree</returns>
        GameTree Deserialize( string serializedTree );
    }
}
