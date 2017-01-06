using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
    /// <summary>
    /// This base class contains code that allows an agent to make moves based on a historical record. This is used most often when resuming
    /// a paused game or when entering a game that's already in progress on a server.
    /// </summary>
    public abstract class AgentBase : IAgent
    {
        /// <summary>
        /// Gets the game that this agent's player is playing in.
        /// </summary>
        protected GameInfo Game { get; private set; }
        /// <summary>
        /// Gets the player that this agent makes moves for.
        /// </summary>
        protected Player Player { get; private set; }
        
        public abstract IllegalMoveHandling HowToHandleIllegalMove { get; }

       
        public void GameBegins(Player player, GameInfo game)
        {
            Player = player;
            Game = game;
        }

        public abstract void PleaseMakeAMove();
    }
}
