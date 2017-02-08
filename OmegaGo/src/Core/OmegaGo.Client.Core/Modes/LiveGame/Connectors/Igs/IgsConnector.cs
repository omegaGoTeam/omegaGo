using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Igs;

namespace OmegaGo.Core.Modes.LiveGame.Connectors.Igs
{
    /// <summary>
    /// Connects the IgsConnection to a specific game
    /// </summary>
    public class IgsConnector : IRemoteConnector
    {
        private readonly IgsGameInfo _gameInfo;

        private readonly Dictionary<int, Move> _storedMoves = new Dictionary<int, Move>();

        public IgsConnector(IgsGameInfo gameInfo)
        {
            _gameInfo = gameInfo;
        }

        /// <summary>
        /// Unique identification of the game
        /// </summary>
        public int GameId => _gameInfo.IgsIndex;

        public void IncomingMove(int moveIndex, Move move)
        {
            
        }
    }
}
