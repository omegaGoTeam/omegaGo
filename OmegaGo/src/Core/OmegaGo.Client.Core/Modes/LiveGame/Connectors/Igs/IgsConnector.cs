using System;
using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.Main.Igs;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Igs;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Modes.LiveGame.Connectors.Igs
{
    /// <summary>
    /// Connects the IgsConnection to a specific game
    /// </summary>
    internal class IgsConnector : IRemoteConnector
    {
        private readonly IgsConnection _connnection;
        private readonly IgsGameController _gameController;

        private bool _handicapSet = false;

        public IgsConnector(IgsGameController igsGameController, IgsConnection connnection)
        {
            _connnection = connnection;
            _gameController = igsGameController;
        }

        /// <summary>
        /// Indicates the handicap for the game
        /// </summary>
        public event EventHandler<int> GameHandicapSet;

        /// <summary>
        /// Unique identification of the game
        /// </summary>
        public int GameId => _gameController.Info.IgsIndex;

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
            var targetPlayer = _gameController.Players[move.WhoMoves];
            var igsAgent = targetPlayer.Agent as IgsAgent;
            if (igsAgent == null) throw new InvalidOperationException("Server sent a move for non-IGS agent");
            igsAgent.IncomingMoveFromServer(moveIndex, move);
        }

        /// <summary>
        /// A undo operation is coming from the server
        /// </summary>
        public void IncomingUndoFromServer()
        {
            ( _gameController.Phase as IgsMainPhase )?.Undo();
        }

        /// <summary>
        /// Sets the game's handicap
        /// </summary>
        /// <param name="stoneCount">Number of handicap stones</param>
        public void SetHandicap(int stoneCount)
        {
            GameHandicapSet?.Invoke(this, stoneCount);
            _handicapSet = true;
        }

        /// <summary>
        /// Informs the connection about a performed move
        /// </summary>
        /// <param name="move">Move that was performed</param>
        public void MovePerformed( Move move)
        {
            //ignore IGS-based moves
            if (_gameController.Players[move.WhoMoves].Agent is IgsAgent) return;
            //inform the connection
            _connnection.MadeMove(_gameController.Info, move);
        }
    }
}
