using System;
using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Main.Igs;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Igs;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Events;
using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Modes.LiveGame.Connectors.Igs
{
    /// <summary>
    /// Connects the IgsConnection to a specific game
    /// </summary>
    internal class IgsConnector : BaseConnector, IRemoteConnector, IIgsConnectorServerActions
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
        public event EventHandler LifeDeathReturnToMainForced;
        public event EventHandler LifeDeathUndoDeathMarksRequested;
        public event EventHandler LifeDeathUndoDeathMarksForced;
        public event EventHandler LifeDeathDoneRequested;
        public event EventHandler LifeDeathDoneForced;
        public event EventHandler<Position> LifeDeathKillGroupRequested;
        public event EventHandler<Position> LifeDeathKillGroupForced;
        public event EventHandler MainUndoRequested;
        public event EventHandler MainUndoForced;
        public event EventHandler<IgsTimeControlAdjustmentEventArgs> TimeControlShouldAdjust;
        public event EventHandler<GameScoreEventArgs> GameScoredAndCompleted;

        /// <summary>
        /// Unique identification of the game
        /// </summary>
        public int GameId => _gameController.Info.IgsIndex;

        /// <summary>
        /// Handles incoming move from the server
        /// </summary>
        /// <param name="moveIndex">Index of the move</param>
        /// <param name="move">Move</param>
        public void MoveFromServer(int moveIndex, Move move)
        {
            if (!_handicapSet)
            {
                //there is no handicap for this IGS game
                HandicapFromServer(0);
            }
            var targetPlayer = _gameController.Players[move.WhoMoves];
            var igsAgent = targetPlayer.Agent as IgsAgent;
            if (igsAgent == null) throw new InvalidOperationException("Server sent a move for non-IGS agent");
            igsAgent.MoveFromServer(moveIndex, move);
        }

        /// <summary>
        /// A undo operation is coming from the server
        /// </summary>
        public void UndoFromServer()
        {
            (_gameController.Phase as IgsMainPhase)?.Undo();
        }

        /// <summary>
        /// Sets the game's handicap
        /// </summary>
        /// <param name="stoneCount">Number of handicap stones</param>
        public void HandicapFromServer(int stoneCount)
        {
            // TODO Petr: Can Handicap info arrive before HandicapPlacement starts?
            GameHandicapSet?.Invoke(this, stoneCount);
            _handicapSet = true;
        }

        /// <summary>
        /// Informs the connection about a performed move
        /// </summary>
        /// <param name="move">Move that was performed</param>
        public void MovePerformed(Move move)
        {
            //ignore IGS-based moves
            if (_gameController.Players[move.WhoMoves].Agent is IgsAgent) return;
            //inform the connection
            _connnection.MadeMove(_gameController.Info, move);
        }

        /// <summary>
        /// Receives and handles resignation from server
        /// </summary>
        /// <param name="resigningPlayerColor">Color of the resigning player</param>
        public void ResignationFromServer(StoneColor resigningPlayerColor)
        {
            var player = _gameController.Players[resigningPlayerColor];
            var igsAgent = player.Agent as IgsAgent;
            if (igsAgent == null) throw new ArgumentException("Resignation from server was not for an IGS player", nameof(resigningPlayerColor));
            igsAgent.ResignationFromServer();
        }

        /// <summary>
        /// Server indicates that it wants to change the game phase
        /// </summary>
        /// <param name="gamePhase">Game phase type to start</param>
        public void SetPhaseFromServer(GamePhaseType gamePhase)
        {
            _gameController.SetPhase(gamePhase);
        }

        public void TimeControlAdjustment(IgsTimeControlAdjustmentEventArgs igsTimeControlAdjustmentEventArgs)
        {
            TimeControlShouldAdjust?.Invoke(this, igsTimeControlAdjustmentEventArgs);
        }

        public void ForceLifeDeathKillGroup(Position deadPosition)
        {
            LifeDeathKillGroupForced?.Invoke(this, deadPosition);
        }

        public void ForceLifeDeathUndoDeathMarks()
        {
            LifeDeathUndoDeathMarksForced?.Invoke(this, EventArgs.Empty);
        }

        public void ScoreGame(GameScoreEventArgs gameScoreEventArgs)
        {
            GameScoredAndCompleted?.Invoke(this, gameScoreEventArgs);
        }

        public void ForceMainUndo()
        {
            MainUndoForced?.Invoke(this, EventArgs.Empty);
        }
    }
}
