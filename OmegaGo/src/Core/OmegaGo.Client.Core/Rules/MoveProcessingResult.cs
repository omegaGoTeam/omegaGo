﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Represents the result of a move made by a player.
    /// </summary>
    public class MoveProcessingResult
    {
        /// <summary>
        /// The legality of the move.
        /// </summary>
        public MoveResult Result;
        
        /// <summary>
        /// List of prisoners/captured stones.
        /// </summary>
        public List<Position> Captures;

        /// <summary>
        /// The new state of game board. 
        /// If the move is legal, game board contains the recently placed stone.
        /// The board does not contain prisoners.
        /// </summary>
        public StoneColor[,] NewBoard;
        
    }

}