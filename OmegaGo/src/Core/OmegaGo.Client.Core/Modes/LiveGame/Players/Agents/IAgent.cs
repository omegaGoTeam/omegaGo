using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players.AI;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    //TODO documentation information is outdated
    /// <summary>
    /// An agent makes moves for a player when this is requested by a game controller.
    /// 
    /// <para>
    /// 
    /// An agent is a class that each <see cref="GamePlayer"/> must refer to. An agent's role is to supply moves made by the player whenever
    /// the game controller demands it. There are several different agents: the <see cref="AiAgent"/> makes moves for an AI program,
    /// the <see cref="AgentBase"/> makes moves for a remote player whose moves are given to us by the server, and then there are GUI
    /// agents (not part of this DLL library) that make moves made when the local player clicks on the game board.
    /// </para>
    /// 
    /// <para>
    /// Making a move, in general, takes a lot of time. The <see cref="AiAgent"/> will usually take about one second to make a move, and
    /// human players often take even longer, perhaps even twenty minutes in some games. Therefore, the way this works is that 
    /// the <see cref="GameController"/> calls the method <see cref="PleaseMakeAMove"/> on an agent, and then, at unspecified time, 
    /// the agent calls <see cref="GameController.MakeMove(GamePlayer, Move)"/> back on the controller.  
    /// </para>    
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Fired when the agent tries to place a stone on a position
        /// </summary>
        event EventHandler<Position> PlaceStone;

        /// <summary>
        /// Fired when the agent resigns
        /// </summary>
        event EventHandler Resign;

        /// <summary>
        /// Fired when the agent passes
        /// </summary>
        event EventHandler Pass;

        /// <summary>
        /// Gets the type of the agent
        /// </summary>
        AgentType Type { get; }

        /// <summary>
        /// Gets the stone color this player uses
        /// </summary>
        StoneColor Color { get; }

        /// <summary>
        /// Indicates how the agent's illegal moves should be handled
        /// </summary>
        IllegalMoveHandling IllegalMoveHandling { get; }

        /// <summary>
        /// Informs the agent, that a move was confirmed
        /// </summary>
        /// <param name="move">Move</param>
        void MovePerformed(Move move);

        /// <summary>
        /// Informs the agent that his last move was illegal
        /// </summary>
        /// <param name="reason">Reason</param>
        void MoveIllegal(MoveResult reason);

        /// <summary>
        /// Assigns the agent to a game
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="gameState">Game state</param>
        void AssignToGame(GameInfo gameInfo, IGameState gameState);

        /// <summary>
        /// Informs the agent that the game is on
        /// </summary>
        void GameInitialized();

        /// <summary>
        /// Informs the agent that the game phase changed
        /// </summary>
        void GamePhaseChanged(GamePhaseType phase);

        /// <summary>
        /// Informs the agent that he is on turn
        /// </summary>
        void OnTurn();

        /// <summary>
        /// Requests the player to make a move
        /// </summary>
        void PleaseMakeAMove();
    }
}
