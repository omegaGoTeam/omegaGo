using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    /// <summary>
    /// An agent makes moves for a player when this is requested by a game controller.
    /// 
    /// <para>
    /// An agent is a class that each <see cref="GamePlayer"/> must refer to. An agent's role is to supply moves made by the player whenever
    /// the game controller demands it. There are several different agents: the <see cref="AIAgent"/> makes moves for an AI program,
    /// the <see cref="ObsoleteOnlineAgent"/> makes moves for a remote player whose moves are given to us by the server, and then there are GUI
    /// agents (not part of this DLL library) that make moves made when the local player clicks on the game board.
    /// </para>
    /// 
    /// <para>
    /// Making a move, in general, takes a lot of time. The <see cref="AIAgent"/> will usually take about one second to make a move, and
    /// human players often take even longer, perhaps even twenty minutes in some games. Therefore, the way this works is that 
    /// the <see cref="ObsoleteGameController"/> calls the method <see cref="PleaseMakeAMove"/> on an agent, and then, at unspecified time, 
    /// the agent calls <see cref="ObsoleteGameController.MakeMove(GamePlayer, Move)"/> back on the controller.  
    /// </para>    
    /// </summary>
    public interface IObsoleteAgent
    {
        

        /// <summary>
        /// Using this property (which might be constant for most agents), the agent informs the <see cref="ObsoleteGameController"/>
        /// how it should proceed if the agent supplies an illegal move. Ideally, agents should not give illegal moves, but especially
        /// weaker AI's sometimes will do so.
        /// </summary>
        IllegalMoveHandling HowToHandleIllegalMove { get; }

        /// <summary>
        /// Called by the game controller when the GAME begins telling the agent that he is controlling the PLAYER.
        /// </summary>
        /// <param name="player">The player the agent is controlling.</param>
        /// <param name="game">The game that player is playing in.</param>
        void GameBegins(GamePlayer player, ObsoleteGameInfo game);

        /// <summary>
        /// Called by the game controller when it requests this agent to make a move. The move may be made before this method 
        /// returns or after that - it doesn't matter.
        /// </summary>
        void PleaseMakeAMove();
    }
}
