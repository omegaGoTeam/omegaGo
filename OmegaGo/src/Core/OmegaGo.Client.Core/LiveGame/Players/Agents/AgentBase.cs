using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.State;
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
        /// Indicates that the player tried to place a stone
        /// </summary>
        public event AgentEventHandler<Position> PlaceStone;       

        /// <summary>
        /// Indicates that the player resigned
        /// </summary>
        public event AgentEventHandler Resigned;

        /// <summary>
        /// Indicates that the player passed
        /// </summary>
        public event AgentEventHandler Pass;       

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
        public IGameState GameState { get; private set; }

        /// <summary>
        /// Type of the agent
        /// </summary>
        public abstract AgentType Type { get; }

        /// <summary>
        /// Illegal move handling
        /// </summary>
        public abstract IllegalMoveHandling IllegalMoveHandling { get; }


        public virtual void PleaseMakeAMove()
        {
        }

        public abstract void MoveIllegal(MoveResult moveResult);

        public virtual void GameInitialized()
        {
        }

        public virtual void GamePhaseChanged(GamePhaseType phase) { }

        public void AssignToGame(GameInfo gameInfo, IGameState gameState)
        {
            GameInfo = gameInfo;
            GameState = gameState;
            WhenAssignedToGame();
        }    

        protected virtual void WhenAssignedToGame()
        {

        }

        protected void OnPlaceStone( Position position )
        {
            PlaceStone?.Invoke(this, position);
        }

        protected void OnPass()
        {
            Pass?.Invoke(this);
        }

        protected void OnResign()
        {
            Resigned?.Invoke(this);
        }

        public virtual void MoveUndone()
        {

        }

        public virtual void MovePerformed(Move move)
        {
        }
    }
}
