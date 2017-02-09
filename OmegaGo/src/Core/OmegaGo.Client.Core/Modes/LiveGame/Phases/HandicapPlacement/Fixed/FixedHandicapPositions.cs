using System;
using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Fixed
{
    public static class FixedHandicapPositions
    {
        public const int MaxFixedHandicap9 = 5;
        public const int MaxFixedHandicap13 = 9;
        public const int MaxFixedHandicap19 = 9;

        public static readonly Position[] FixedHandicapPositions9 = new Position[] { new Position(6, 6),
                                                                     new Position(2, 2),
                                                                     new Position(6, 2),
                                                                     new Position(2, 6),
                                                                     new Position(4, 4) };

        public static readonly Position[] FixedHandicapPositions13 = new Position[] { new Position(9, 9),
                                                                      new Position(3, 3),
                                                                      new Position(9, 3),
                                                                      new Position(3, 9),
                                                                      new Position(9, 6),
                                                                      new Position(3, 6),
                                                                      new Position(6, 9),
                                                                      new Position(6, 3),
                                                                      new Position(6, 6) };

        public static readonly Position[] FixedHandicapPositions19 = new Position[] { new Position(15, 15),
                                                                      new Position(3, 3),
                                                                      new Position(15, 3),
                                                                      new Position(3, 15),
                                                                      new Position(15, 9),
                                                                      new Position(3, 9),
                                                                      new Position(9, 15),
                                                                      new Position(9, 3),
                                                                      new Position(9, 9) };

        /// <summary>
        /// Returns the positions of handicap stones
        /// </summary>
        /// <param name="size">Size of the game board</param>
        /// <param name="handicap">Handicap stone count</param>
        /// <returns>Positions of handicap stones</returns>
        public static IEnumerable<Position> GetHandicapStonePositions(GameBoardSize size, int handicap)
        {
            if ( !size.IsSquare) throw new ArgumentOutOfRangeException(nameof(size), "Invalid game board size for fixed handicap");
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

            if ( handicap > sourceArray.Length)
                throw new ArgumentOutOfRangeException(nameof(handicap),"Too many handicap stones requested");

            return sourceArray.Take(handicap);
        }
    }
}
