using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    /// <summary>
    /// This base class contains code that allows an agent to make moves based on a historical record. This is used most often when resuming
    /// a paused game or when entering a game that's already in progress on a server.
    /// </summary>
    public abstract class ObsoleteAgentBase : IObsoleteAgent
    {
        /// <summary>
        /// Gets the game that this agent's player is playing in.
        /// </summary>
        protected ObsoleteGameInfo Game { get; private set; }
        /// <summary>
        /// Gets the player that this agent makes moves for.
        /// </summary>
        protected GamePlayer Player { get; private set; }
        
        public abstract IllegalMoveHandling HowToHandleIllegalMove { get; }

       
        public void GameBegins(GamePlayer player, ObsoleteGameInfo game)
        {
            Player = player;
            Game = game;
        }

        public abstract void PleaseMakeAMove();
    }
}
