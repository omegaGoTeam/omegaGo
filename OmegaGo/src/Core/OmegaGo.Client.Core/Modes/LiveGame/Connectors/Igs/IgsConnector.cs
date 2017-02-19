using System;
using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Igs;
using OmegaGo.Core.Online.Igs;

namespace OmegaGo.Core.Modes.LiveGame.Connectors.Igs
{
    /// <summary>
    /// Connects the IgsConnection to a specific game
    /// </summary>
    public class IgsConnector : IRemoteConnector
    {
        private readonly IgsConnection _connnection;
        private readonly IgsGameInfo _gameInfo;
        private readonly PlayerPair _players;

        private bool _handicapSet = false;

        public IgsConnector(IgsConnection connnection, IgsGameInfo gameInfo, PlayerPair players)
        {
            _connnection = connnection;
            _gameInfo = gameInfo;
            _players = players;
        }

        /// <summary>
        /// Unique identification of the game
        /// </summary>
        public int GameId => _gameInfo.IgsIndex;

        /// <summary>
        /// Handles incoming move from the server
        /// </summary>
        /// <param name="moveIndex">Index of the move</param>
        /// <param name="move">Move</param>
        public void IncomingMoveFromServer(int moveIndex, Move move)
        {
            if (!_handicapSet)
            {
                //there is no handicap for this IGS game
                SetHandicap(0);
            }
            var targetPlayer = _players[move.WhoMoves];
            var igsAgent = targetPlayer.Agent as IgsAgent;
            if (igsAgent == null) throw new InvalidOperationException("Server sent a move for non-IGS agent");
            igsAgent.IncomingMoveFromServer(moveIndex, move);
        }

        /// <summary>
        /// Sets the game's handicap
        /// </summary>
        /// <param name="stoneCount">Number of handicap stones</param>
        public void SetHandicap(int stoneCount)
        {
            // TODO (Petr): This should probably have a guard or not use null coalescing (if GameHandicapSet is not set, something is wrong)
            GameHandicapSet?.Invoke(this, stoneCount);
            _handicapSet = true;
        }

        /// <summary>
        /// Indicates the handicap for the game
        /// </summary>
        public event EventHandler<int> GameHandicapSet;

        /// <summary>
        /// Informs the connection about a performed move
        /// </summary>
        /// <param name="move">Move that was performed</param>
        public void MovePerformed( Move move)
        {
            //ignore IGS based moves
            if (_players[move.WhoMoves].Agent is IgsAgent) return;
            //inform the connection
            _connnection.MadeMove(_gameInfo, move);
        }
    }
}
