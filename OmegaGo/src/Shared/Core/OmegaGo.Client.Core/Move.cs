using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    public class Move
    {
        public bool UnknownMove;
        public Color WhoMoves;
        public Position Coordinates;

        public static Move CreateUnknownMove()
        {
            return new Move() { UnknownMove = true };
        }
        public static Move Create(Color whoMoves, Position where)
        {
            return new Move()
            {
                WhoMoves = whoMoves,
                Coordinates = where
            };
        }
    }
}
