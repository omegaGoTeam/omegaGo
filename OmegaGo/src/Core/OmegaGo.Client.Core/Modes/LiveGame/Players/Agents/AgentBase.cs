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
    public abstract class AgentBase : IAgent
    {
        protected GamePlayer Player { get; }

        /// <summary>
        /// Creates agent
        /// </summary>
        /// <param name="player">Player for which this is an agent</param>
        protected AgentBase(GamePlayer player)
        {
            Player = player;
        }

        public abstract void MovePerformed(Move move, GamePlayer player);

        public abstract void MoveIllegal(MoveResult move);

        public abstract event EventHandler Move;
        public abstract IllegalMoveHandling IllegalMoveHandling { get; }
        public abstract void GameInitialized();

        public abstract void GamePhaseChanged(GamePhaseType phase);

        public abstract void OnTurn();
    }
}
