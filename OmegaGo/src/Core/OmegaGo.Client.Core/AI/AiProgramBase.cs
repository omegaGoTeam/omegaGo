using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Base of AI programs
    /// </summary>
    public abstract class AIProgramBase : IAIProgram
    {
        /// <summary>
        /// Capabilities of the AI
        /// </summary>
        public abstract AICapabilities Capabilities { get; }

        /// <summary>
        /// Requests a move from the AI
        /// </summary>
        /// <param name="gameInformation"></param>
        /// <returns></returns>
        public abstract AIDecision RequestMove(AiGameInformation gameInformation);

        /// <summary>
        /// Gets a hint from the AI
        /// </summary>
        /// <param name="gameInformation"></param>
        /// <returns></returns>
        public virtual AIDecision GetHint(AiGameInformation gameInformation)
        {
            if (!Capabilities.ProvidesHints)
            {
                throw new InvalidOperationException("This AI is incapable of providing hints.");
            }
            return RequestMove(gameInformation);
        }

        /// <summary>
        /// Informs the AI engine that a move was just undone. Stateful AIs (i.e. Fuego) use this.
        /// </summary>
        public virtual void MoveUndone()
        {
            // Stateless AI's don't need to do anything.
        }

        public virtual void YourMoveWasRejected()
        {
            // Stateless AI's don't need to do anything.
        }

        /// <summary>
        ///Informs the AI engine that a move was just made. Stateful AIs (i.e. Fuego) use this.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="gameTree">The game tree.</param>
        /// <param name="informedPlayer">The player who is associated with this AI, not the player who made the move.</param>
        /// <param name="info">Information about the game</param>
        public virtual void MovePerformed(Move move, GameTree gameTree, GamePlayer informedPlayer, GameInfo info)
        {
            // Stateless AI's don't need to do anything.
        }

        /// <summary>
        /// Determines (asynchronously, if possible) all positions that should be marked dead at the beginning of the Life/Death Determination Phase.
        /// </summary>
        /// <param name="gameController"></param>
        /// <exception cref="Exception">Nobody except Fuego supports this.</exception>
        public virtual Task<IEnumerable<Position>> GetDeadPositions(IGameController gameController)
        {
            throw new Exception("Nobody except Fuego supports this.");
        }

        public override string ToString()
        {
            return GetType().Name;
        }


        /// <summary>
        /// Gets the LastNode of the tree. If none exists yet, we are at the very beginning of a game, and we'll use an empty board instead.
        /// </summary>
        protected GameTreeNode GetLastNodeOrEmptyBoard(GameTree gameTree)
        {
            GameTreeNode lastNode = gameTree.LastNode;
            if (lastNode == null)
            {
                GameTreeNode empty = new GameTreeNode(Move.NoneMove);
                empty.BoardState = new GameBoard(gameTree.BoardSize);
                empty.GroupState = new Rules.GroupState(gameTree.Ruleset.RulesetInfo);
                return empty;
            }
            else
            {
                return lastNode;
            }
        }
    }
}
