using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Igs;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Kgs;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.LiveGame.Players.Agents
{
    /// <summary>
    /// An agent makes moves for a player when this is requested by a game controller.
    /// 
    /// <para> 
    /// An agent is a class that each <see cref="GamePlayer"/> must refer to. An agent's role is to supply moves made by the player whenever
    /// the game controller demands it. There are several different agents: the <see cref="AiAgent"/> makes moves for an AI program,
    /// the <see cref="IgsAgent"/> and <see cref="KgsAgent"/>  make moves for a remote player whose moves are given to us by the server, 
    /// and <see cref="HumanAgent"/> makes moves when the local player clicks on the game board.
    /// </para>
    /// 
    /// <para>
    /// Making a move, in general, takes a lot of time. The <see cref="AiAgent"/> will usually take seconds to make a move, and
    /// human players often take even longer, perhaps even twenty minutes in some games. Therefore, the way this works is that 
    /// the <see cref="GameController"/> calls the method <see cref="PleaseMakeAMove"/> on an agent, and then, at unspecified time, 
    /// the agent raises the <see cref="PlaceStone"/> event or <see cref="Pass"/> event which is handled by the controller.
    /// 
    /// More complex communication between players and the game controller is possible, and there are multiple communication channels: events,
    /// method calls, agents and connectors are all related. Please refer to the developer documentation for more information.  
    /// </para>    
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Fired when the agent tries to place a stone on a position
        /// </summary>
        event AgentEventHandler<Position> PlaceStone;

        /// <summary>
        /// Fired when the agent resigns
        /// </summary>
        event AgentEventHandler Resigned;

        /// <summary>
        /// Fired when the agent passes
        /// </summary>
        event AgentEventHandler Pass;

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
        /// Informs the agent that his last move was illegal
        /// </summary>
        /// <param name="moveResult">Reason</param>
        void MoveIllegal(MoveResult moveResult);

        /// <summary>
        /// Assigns the agent to a game
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="gameState">Game state</param>
        void AssignToGame(GameInfo gameInfo, IGameState gameState);

        /// <summary>
        /// Informs the agent that the game is passing through its initialization phase.
        /// </summary>
        void GameInitialized();

        /// <summary>
        /// Informs the agent that the game phase changed
        /// </summary>
        void GamePhaseChanged(GamePhaseType phase);

        /// <summary>
        /// Requests the player to make a move
        /// </summary>
        void PleaseMakeAMove();

        /// <summary>
        /// Informs the agent that the latest move made in the game was just undone.
        /// </summary>
        void MoveUndone();

        /// <summary>
        /// Informs the agent that a move was just made in the game. This may be the agent's own move or the move of the other player.
        /// </summary>
        void MovePerformed(Move move);
    }
}
