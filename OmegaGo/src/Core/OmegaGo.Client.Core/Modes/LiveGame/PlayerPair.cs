﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame
{
    /// <summary>
    /// Represents a pair of players in the game
    /// </summary>
    public class PlayerPair : IEnumerable<GamePlayer>
    {
        /// <summary>
        /// Creates pair of players
        /// </summary>
        /// <param name="black">Black player</param>
        /// <param name="white">White player</param>
        public PlayerPair(GamePlayer black, GamePlayer white)
        {
            Black = black;
            White = white;
        }

        /// <summary>
        /// Black player
        /// </summary>
        public GamePlayer Black { get; }

        /// <summary>
        /// White player
        /// </summary>
        public GamePlayer White { get; }

        /// <summary>
        /// Returns the player by stone color
        /// </summary>
        /// <param name="color">Stone color</param>
        /// <returns>Player</returns>
        public GamePlayer this[StoneColor color]
        {
            get
            {
                switch (color)
                {
                    case StoneColor.Black:
                        return Black;                       
                    case StoneColor.White:
                        return White;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(color), color, null);
                }
            }
        }

        /// <summary>
        /// Enumerates both players
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator<GamePlayer> GetEnumerator()
        {
            yield return Black;
            yield return White;
        }

        /// <summary>
        /// Non-generic enumerator
        /// </summary>
        /// <returns>Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
