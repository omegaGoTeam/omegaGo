﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Represents a request for the AI to make a move. Data in this class provides the AI with all the infromation it needs to decide on what it thinks is the best move.
    /// </summary>
    public class AIPreMoveInformation
    {
        /// <summary>
        /// The player whose turn it is. The AI will make a move for this player.
        /// </summary>
        public StoneColor AIColor { get; }
        /// <summary>
        /// The current full board state (excluding information about Ko). 
        /// </summary>
        public GameBoard Board { get; }
        /// <summary>
        /// How much time does the AI have before it must make a decision. The AI will use this as a guidance,
        /// it may provide its decision earlier or later. If it doesn't provide a decision by this time, the
        /// main program may perform some actions such as ending the game, or asking the player whether
        /// he wishes to continue waiting.
        /// </summary>
        public TimeSpan TimeLimit { get; }
        /// <summary>
        /// Level of strength the AI should demonstrate. Levels go from 1 (lowest) to 10 (highest).
        /// </summary>
        public int Difficulty { get; }
        public IEnumerable<Move> History { get; }

        /// <summary>
        /// Creates a new structure that gives the AI information it needs to make a move.
        /// </summary>
        /// <param name="aiColor">The player whose turn it is. The AI will make a move for this player.</param>
        /// <param name="board">The current full board state (excluding information about Ko). </param>        
        /// <param name="timeLimit">How much time does the AI have before it must make a decision.</param>
        /// <param name="difficulty">How powerful should the AI be.</param>
        /// <param name="history">What moves were played previously in the game, starting with the first.</param>
        public AIPreMoveInformation(StoneColor aiColor, GameBoard board,TimeSpan timeLimit, int difficulty, IEnumerable<Move> history)
        {
            Difficulty = difficulty;
            AIColor = aiColor;
            History = history;
            Board = board;
            TimeLimit = timeLimit;
        }
    }
}
