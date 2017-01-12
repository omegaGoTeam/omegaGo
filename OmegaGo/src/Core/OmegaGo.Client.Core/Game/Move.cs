using System;
using System.Collections.Generic;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Represents the move of a player, i.e. the result of a single turn of a game of Go.
    /// </summary>
    public class Move
    {
        /// <summary>
        /// There are two kinds of moves - placing a stone; and passing.
        /// </summary>
        public MoveKind Kind;
        /// <summary>
        /// Color of the player who made this move.
        /// </summary>
        public StoneColor WhoMoves;
        /// <summary>
        /// If this move is the placing of a stone, then these are the coordinates of the placement. Otherwise, this field has no meaning.
        /// </summary>
        public Position Coordinates;
        /// <summary>
        /// List of intersections at which a capture was made by this move.
        /// </summary>
        public List<Position> Captures { get; } = new List<Position>();

        /// <summary>
        /// Prevents a default instance of the <see cref="Move"/> class from being created. Use the 
        /// <see cref="PlaceStone(StoneColor, Position)"/> and <see cref="Pass(StoneColor)"/>  
        /// factory methods instead.
        /// </summary>
        private Move()
        {

        }

        /// <summary>
        /// A none move
        /// </summary>
        /// <returns></returns>
        public static Move NoneMove { get; } = new Move() { WhoMoves = StoneColor.None };

        public static Move Pass(StoneColor whoMoves)
        {
            return new Move()
            {
                WhoMoves = whoMoves,
                Kind = MoveKind.Pass
            };
        }
        public static Move PlaceStone(StoneColor whoMoves, Position where)
        {
            return new Move()
            {
                WhoMoves = whoMoves,
                Coordinates = where,
                Kind = MoveKind.PlaceStone
            };
        }
        public override string ToString()
        {
            if (Kind == MoveKind.Pass) return "PASS";
            if (Kind == MoveKind.PlaceStone) return Coordinates.ToString();
            throw new Exception("This move kind does not exist.");
        }
    }
}
