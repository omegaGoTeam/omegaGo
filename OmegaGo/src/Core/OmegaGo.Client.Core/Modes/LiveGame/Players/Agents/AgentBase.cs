using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    /// <summary>
    /// Base for agents
    /// </summary>
    public abstract class AgentBase : IAgent
    {
        /// <summary>
        /// Creates agent
        /// </summary>
        /// <param name="color">Player for which this is an agent</param>
        protected AgentBase(StoneColor color )
        {
            Color = color;
        }

        /// <summary>
        /// Player color
        /// </summary>
        public StoneColor Color { get; }

        /// <summary>
        /// Game info
        /// </summary>
        protected GameInfo GameInfo { get; private set; }

        /// <summary>
        /// Game state
        /// </summary>
        protected IGameState GameState { get; private set; }

        /// <summary>
        /// Type of the agent
        /// </summary>
        public abstract AgentType Type { get; }

        /// <summary>
        /// Illegal move handling
        /// </summary>
        public abstract IllegalMoveHandling IllegalMoveHandling { get; }

        public event EventHandler<Position> PlaceStone;

        public event EventHandler Resign;

        public abstract void MoveIllegal(MoveResult move);

        public virtual void GameInitialized()
        {
        }

        public virtual void GamePhaseChanged(GamePhaseType phase) { }

        public virtual void OnTurn()
        {
        }

        public void AssignToGame(GameInfo gameInfo, IGameState gameState)
        {
            GameInfo = gameInfo;
            GameState = gameState;
        }    

        protected virtual void OnPlaceStone( Position position )
        {
            PlaceStone?.Invoke(this, position);
        }

        protected virtual void OnResign()
        {
            Resign?.Invoke(this, EventArgs.Empty);
        }

        public virtual void MovePerformed(Move move)
        {
            
        }
    }
}
