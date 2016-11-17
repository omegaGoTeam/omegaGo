using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
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
        public List<Position> Captures = new List<Position>();
        
        public static Move Create(StoneColor whoMoves, Position where)
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
            else if (Kind == MoveKind.PlaceStone) return Coordinates.ToString();
            else throw new Exception("This move kind does not exist.");
        }
    }

    /// <summary>
    /// There are two kinds of moves - placing a stone; and passing.
    /// </summary>
    public enum MoveKind
    {
        /// <summary>
        /// The most common move - a player places a stone on the board.
        /// </summary>
        PlaceStone,
        /// <summary>
        /// The other move - a player passes to signal that they think the game is over.
        /// </summary>
        Pass
    }
}
