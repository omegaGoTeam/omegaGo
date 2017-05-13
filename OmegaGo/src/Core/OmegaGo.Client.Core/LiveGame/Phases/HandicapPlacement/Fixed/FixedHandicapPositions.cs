using System;
using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Fixed
{
    /// <summary>
    /// Provides fixed handicap positions
    /// </summary>
    internal static class FixedHandicapPositions
    {
        /// <summary>
        /// Fixed handicap positions for 9x9 board
        /// </summary>
        private static readonly Position[] FixedHandicapPositions9 =
        {
            new Position(6, 6),
            new Position(2, 2),
            new Position(2, 6),
            new Position(6, 2),
            new Position(4, 4),
            new Position(2, 4),
            new Position(6, 4),
            new Position(4, 6),
            new Position(4, 2)
        };

        /// <summary>
        /// Fixed handicap positions 13x13 board
        /// </summary>
        private static readonly Position[] FixedHandicapPositions13 =
        {
            new Position(9, 9),
            new Position(3, 3),
            new Position(3, 9),
            new Position(9, 3),
            new Position(6, 6),
            new Position(3, 6),
            new Position(9, 6),
            new Position(6, 9),
            new Position(6, 3)
        };

        /// <summary>
        /// Fixed handic positions for 19x19 boards
        /// </summary>
        private static readonly Position[] FixedHandicapPositions19 =
        {
            new Position(15, 15),
            new Position(3, 3),
            new Position(3, 15),
            new Position(15, 3),
            new Position(9, 9),
            new Position(3, 9),
            new Position(15, 9),
            new Position(9, 15),
            new Position(9, 3)
        };

        /// <summary>
        /// Returns the positions of handicap stones
        /// </summary>
        /// <param name="size">Size of the game board</param>
        /// <param name="handicap">Handicap stone count</param>
        /// <returns>Positions of handicap stones</returns>
        public static IEnumerable<Position> GetHandicapStonePositions(GameBoardSize size, int handicap)
        {
            if (!size.IsSquare)
                throw new ArgumentOutOfRangeException(nameof(size), "Invalid game board size for fixed handicap");
            Position[] sourceArray = null;
            switch (size.Width)
            {
                case 9:
                    {
                        sourceArray = FixedHandicapPositions9;
                        break;
                    }
                case 13:
                    {
                        sourceArray = FixedHandicapPositions13;
                        break;
                    }
                case 19:
                    {
                        sourceArray = FixedHandicapPositions19;
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(size), "Invalid game board size for fixed handicap");
            }

            if (handicap > sourceArray.Length)
                throw new ArgumentOutOfRangeException(nameof(handicap), "Too many handicap stones requested");

            if (handicap == 6 || handicap == 8) 
                sourceArray = sourceArray.Take(4).Concat(sourceArray.Skip(5)).ToArray();

            return sourceArray.Take(handicap);
            
        }

        /// <summary>
        /// Returns the maximum fixed handicap for a given board size
        /// </summary>
        /// <param name="size">Game board size</param>
        /// <returns>Maximum fixed handicap value</returns>
        public static int GetMaximumHandicap(GameBoardSize size)
        {
            if (!size.IsSquare)
                throw new ArgumentOutOfRangeException(nameof(size), "Invalid game board size for fixed handicap");
            switch (size.Width)
            {
                case 9:
                    return FixedHandicapPositions9.Length;
                case 13:
                    return FixedHandicapPositions13.Length;
                case 19:
                    return FixedHandicapPositions19.Length;
                default:
                    throw new ArgumentOutOfRangeException(nameof(size), "Invalid game board size for fixed handicap");
            }
        }

        /// <summary>
        /// Returns game board sizes supported for fixed handicap
        /// </summary>
        /// <returns>Supported game board sizes</returns>
        public static IEnumerable<GameBoardSize> GetSupportedBoardSizes() =>
            new[] { new GameBoardSize(9), new GameBoardSize(13), new GameBoardSize(19) };
    }
}