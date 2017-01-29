﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame
{
    /// <summary>
    /// Provides the state of a game
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Gets the player on turn
        /// </summary>
        GamePlayer TurnPlayer { get; }

        /// <summary>
        /// Gets the tree of the game
        /// </summary>
        GameTree GameTree { get; }

        /// <summary>
        /// Gets the current game tree node
        /// </summary>
        GameTreeNode CurrentNode { get; }

        /// <summary>
        /// Gets the current game phase
        /// </summary>
        GamePhaseType Phase { get; }

        /// <summary>
        /// Gets the number of moves that were played
        /// </summary>
        int NumberOfMoves { get; }
    }
}