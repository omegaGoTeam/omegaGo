using System;
using System.Collections;
using System.Collections.Generic;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players
{
    /// <summary>
    /// Represents a pair of players in the game
    /// </summary>
    public class PlayerPair : IEnumerable<GamePlayer>
    {
        /// <summary>
        /// Creates pair of players in a game
        /// </summary>
        /// <param name="firstPlayer">First player</param>
        /// <param name="secondPlayer">Second player</param>
        public PlayerPair(GamePlayer firstPlayer, GamePlayer secondPlayer)
        {
            if (firstPlayer == null) throw new ArgumentNullException(nameof(firstPlayer));
            if (secondPlayer == null) throw new ArgumentNullException(nameof(secondPlayer));
            if (firstPlayer.Info.Color == StoneColor.None) throw new ArgumentException("First player has none color", nameof(firstPlayer));
            if (secondPlayer.Info.Color == StoneColor.None) throw new ArgumentException("Second player has none color", nameof(secondPlayer));
            if (firstPlayer.Info.Color == secondPlayer.Info.Color) throw new ArgumentException("Both players in a pair have the same color.");

            //set the players according to their color
            if (firstPlayer.Info.Color == StoneColor.Black)
            {
                Black = firstPlayer;
                White = secondPlayer;
            }
            else
            {
                Black = secondPlayer;
                White = firstPlayer;
            }
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
        /// Gets the local player that's playing on this device. This should only be used in multiplayer games.
        /// If no player is local (i.e. we are observing a game), then return null.
        /// </summary>
        public GamePlayer Local => White.IsLocal ? White : (Black.IsLocal ? Black : null);

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
        /// Gets the oponent of the given player
        /// </summary>
        /// <param name="player">Player to get opponent of</param>
        /// <returns>Opponent of player</returns>
        public GamePlayer GetOpponentOf(GamePlayer player)
        {
            if (player == Black) return White;
            if (player == White) return Black;
            throw new ArgumentOutOfRangeException(nameof(player), "This player is not a player in this game");
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
        public override string ToString()
        {
            return White + " v. " + Black;
        }
    }
}
