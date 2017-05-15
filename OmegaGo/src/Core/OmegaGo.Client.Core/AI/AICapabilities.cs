using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Contains information about the capabilities of an AI program. Some programs, for example,
    /// are unable to handle non-square boards or to provide hints.
    /// </summary>
    public class AICapabilities
    {
        /// <summary>
        /// Creates AI capabilities
        /// </summary>
        /// <param name="handlesNonSquareBoards">Non-square board handling</param>
        /// <param name="providesHints">Does AI provide hints</param>
        /// <param name="minSize">Minimum supported size</param>
        /// <param name="maxSize">Maximum supported size</param>
        /// <param name="providesFinalEvaluation">See <see cref="ProvidesFinalEvaluation"/>.</param>
        public AICapabilities(bool handlesNonSquareBoards, bool providesHints, int minSize, int maxSize, bool providesFinalEvaluation = false)
        {
            HandlesNonSquareBoards = handlesNonSquareBoards;
            ProvidesHints = providesHints;
            MinimumBoardSize = minSize;
            //omegaGo supprts at most 52x52 boards
            MaximumBoardSize = Math.Min(maxSize, 52);
            ProvidesFinalEvaluation = providesFinalEvaluation;
        }

        /// <summary>
        /// Gets a value indicating whether the AI can handle non-square boards.
        /// </summary>
        public bool HandlesNonSquareBoards { get; }

        /// <summary>
        /// Gets a value indicating whether the AI can provide hints as to what move should be made (or if the player should resign). 
        /// </summary>
        public bool ProvidesHints { get; }

        /// <summary>
        /// Gets the minimum size of the board the AI can play at. The board must have at least this size in both width
        /// and height for the AI to work.
        /// </summary>
        public int MinimumBoardSize { get; }
        /// <summary>
        /// Gets the maximum size of the board the AI can play at. The board must have at most this size in both width
        /// and height for the AI to work. 
        /// </summary>
        public int MaximumBoardSize { get; }

        /// <summary>
        /// Gets a value indicating whether the AI can help determine life and death status of stones at the end of a game. Only Fuego can do this.
        /// </summary>
        public bool ProvidesFinalEvaluation { get; }
    }
}
