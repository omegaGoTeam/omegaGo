using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
    /// <summary>
    /// An agent makes moves for a player when this is requested by a game controller.
    /// 
    /// <para>
    /// An agent is a class that each <see cref="Player"/> must refer to. An agent's role is to supply moves made by the player whenever
    /// the game controller demands it. There are several different agents: the <see cref="AIAgent"/> makes moves for an AI program,
    /// the <see cref="OnlineAgent"/> makes moves for a remote player whose moves are given to us by the server, and then there are GUI
    /// agents (not part of this DLL library) that make moves made when the local player clicks on the game board.
    /// </para>
    /// 
    /// <para>
    /// Making a move, in general, takes a lot of time. The <see cref="AIAgent"/> will usually take about one second to make a move, and
    /// human players often take even longer, perhaps even twenty minutes in some games. Therefore, the <see cref="RequestMoveAsync"/>
    /// should usually return immediately, and it will be awaited by the game controller. 
    /// </para>    
    /// 
    /// <para>
    /// Petr: For now, I'm using BufferBlock in GUI agents to transfer the moves from the user interface to the agent. This is not ideal, and
    /// it would perhaps be better to use a simple condition variable. Someone (possibly me) should look into that.
    /// </para>
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Using this property (which might be constant for most agents), the agent informs the <see cref="GameController"/>
        /// how it should proceed if the agent supplies an illegal move. Ideally, agents should not give illegal moves, but especially
        /// weaker AI's sometimes will do so.
        /// </summary>
        IllegalMoveHandling HowToHandleIllegalMove { get; }
        /// <summary>
        /// This is usually called by the server connection and informs this agent that the next time it's requested to make a move at
        /// the specified turn number, it should immediately make the move specified by this call instead of how it would usually do it.
        /// This is used to fill out the game history when resuming an online game, for example.
        /// </summary>
        /// <param name="moveIndex">The 1-based turn number.</param>
        /// <param name="move">The move to make at the given turn number.</param>
        void ForceHistoricMove(int moveIndex, Move move);

        /// <summary>
        /// GUI interface will call this method on a GUI agent when the user requests that a stone be placed at a position.
        /// </summary>
        /// <param name="color">The color of the placed stone.</param>
        /// <param name="selectedPosition">The position to place the stone on.</param>
        void Click(StoneColor color, Position selectedPosition);
        /// <summary>
        /// GUI interface will call this method on a GUI agent when the user requests that a player passes.
        /// </summary>
        /// <param name="color">The color of this agent's player.</param>
        void ForcePass(StoneColor color);

        /// <summary>
        /// Called by the game controller when the GAME begins telling the agent that he is controlling the PLAYER.
        /// </summary>
        /// <param name="player">The player the agent is controlling.</param>
        /// <param name="game">The game that player is playing in.</param>
        void GameBegins(Player player, Game game);

        /// <summary>
        /// Called by the game controller when it requests this agent to make a move. The move may be made before this method 
        /// returns or after that - it doesn't matter.
        /// </summary>
        void PleaseMakeAMove();
    }
}
