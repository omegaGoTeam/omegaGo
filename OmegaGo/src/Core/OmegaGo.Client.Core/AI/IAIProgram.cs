using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Represents an AI program that can intelligently play Go by making moves in response to requests by
    /// the controller application. DO NOT implement this interface directly, instead use the <see cref="AIProgramBase"/> as your base class. 
    /// </summary>
    public interface IAIProgram
    {
        /// <summary>
        /// Gets a structure that informs the core what actions, rulesets and features the AI is capable of.
        /// </summary>
        AICapabilities Capabilities { get; }

        /// <summary>
        /// Asks the AI to make a move or resign, synchronously. It is guaranteed that this method
        /// will be called in order, but it may be called on white or black. The AI
        /// should check the history whether it agrees with its own. In case of a conflict,
        /// the history provided in the pre-move information takes precedence and the AI might need to erase its own history. Undo's are
        /// given to the AI using additional methods. Most AI's are stateless and won't need this.
        /// </summary>
        /// <param name="gameInformation">Information the AI might need.</param>
        /// <returns>Decision</returns>
        AIDecision RequestMove (AiGameInformation gameInformation);

        /// <summary>
        /// Asks the AI to tell us its best move for the given situation. This method must not have side-effects or
        /// be affected by the state of the class.
        /// </summary>
        /// <param name="gameInformation">Information the AI might need.</param>
        AIDecision GetHint(AiGameInformation gameInformation);

        void MoveUndone();


        /// <summary>
        /// Informs the AI engine that a move was just made in the game. This may be the agent's own move or the move of the other player.
        /// </summary>
        /// <param name="move">We're currently not using this move becasue we can get this information from the GameTree, but some future AI's might want it for simplicity.</param>
        /// <param name="gameTree">The game tree.</param>
        /// <param name="informedPlayer">The player associated with this AI, not the player who made the move.</param>
        /// <param name="info">Information about the game.</param>
        void MovePerformed(Move move, GameTree gameTree, GamePlayer informedPlayer, GameInfo info);

        /// <summary>
        /// Determines (asynchronously, if possible) all positions that should be marked dead at the beginning of the Life/Death Determination Phase.
        /// </summary>
        /// <param name="gameController"></param>
        Task<IEnumerable<Position>> GetDeadPositions(IGameController gameController);

        void YourMoveWasRejected();
    }
}
