using System;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.LiveGame.Connectors;
using OmegaGo.Core.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs;

namespace OmegaGo.Core.Modes.LiveGame.Connectors.UI
{
    public class UiConnector : IGameConnector, IUiConnectorActions
    {
        private readonly IGameController _gameController;

        public UiConnector(IGameController gameController)
        {
            _gameController = gameController;
            InitAgents();
        }

#pragma warning disable CS0067
        public event EventHandler LifeDeathReturnToMainForced;
        public event EventHandler LifeDeathUndoDeathMarksRequested;
        public event EventHandler LifeDeathUndoDeathMarksForced;
        public event EventHandler LifeDeathDoneRequested;
        public event EventHandler LifeDeathDoneForced;
        public event EventHandler<Position> LifeDeathKillGroupRequested;
        public event EventHandler<Position> LifeDeathKillGroupForced;
        public event EventHandler<Position> LifeDeathRevivifyGroupForced;
        public event EventHandler MainUndoRequested;
        public event EventHandler MainUndoForced;
#pragma warning restore CS0067

        /// <summary>
        ///     Occurs when any AI involved in the game sends a line of information to the user interface.
        /// </summary>
        public event EventHandler<string> AiLog;

        public void MovePerformed(Move move)
        {
            MoveWasPerformed?.Invoke(this, move);
        }

        /// <summary>
        ///     Occurs just after a move is made in the game.
        /// </summary>
        public event EventHandler<Move> MoveWasPerformed;

        /// <summary>
        ///     Tries to make a move for the turn player only if it is a human player
        /// </summary>
        /// <param name="position">Position to play</param>
        public void MakeMove(Position position)
        {
            GetHumanAgentOnTurn()?.PlaceStone(position);
        }

        /// <summary>
        ///     Resigns the turn player
        /// </summary>
        public void Resign()
        {
            if (_gameController.TurnPlayer.IsHuman)
            {
                GetHumanAgentOnTurn()?.Resign();
            }
            else
            {
                GamePlayer human = _gameController.Players.FirstOrDefault(pl => pl.IsHuman);
                (human?.Agent as IHumanAgentActions)?.Resign();
            }
        }

        /// <summary>
        ///     Makes the player pass
        /// </summary>
        public void Pass()
        {
            GetHumanAgentOnTurn()?.Pass();
        }

        public void RequestLifeDeathDone()
        {
            LifeDeathDoneRequested?.Invoke(this, EventArgs.Empty);
        }

        public void ForceLifeDeathReturnToMain()
        {
            LifeDeathReturnToMainForced?.Invoke(this, EventArgs.Empty);
        }

        public void RequestLifeDeathUndoDeathMarks()
        {
            LifeDeathUndoDeathMarksRequested?.Invoke(this, EventArgs.Empty);
        }

        public void RequestLifeDeathKillGroup(Position selectedPosition)
        {
            LifeDeathKillGroupRequested?.Invoke(this, selectedPosition);
        }

        public void RequestMainUndo()
        {
            MainUndoRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Initializes agents
        /// </summary>
        private void InitAgents()
        {
            foreach (var aiAgent in this._gameController.Players.Select(p => p.Agent).OfType<AiAgent>())
            {
                aiAgent.AiNote += AiAgent_AiNote;
            }
        }

        /// <summary>
        ///     AI agent log
        /// </summary>
        /// <param name="agent">Agent</param>
        /// <param name="note">Note to add</param>
        private void AiAgent_AiNote(IAgent agent, string note)
        {
            string aiLogLine = agent.Color.ToIgsCharacterString() + ": " + note;
            AiLog?.Invoke(this, aiLogLine);
        }

        /// <summary>
        ///     Gets the human player currently on turn
        /// </summary>
        /// <returns>Human agent actions</returns>
        private IHumanAgentActions GetHumanAgentOnTurn() => this._gameController.TurnPlayer.Agent as IHumanAgentActions;
    }
}