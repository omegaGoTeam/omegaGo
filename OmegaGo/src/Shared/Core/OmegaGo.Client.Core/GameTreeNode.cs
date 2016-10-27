using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    public sealed class GameTreeNode
    {
        // Information taken from official SGF file definition
        // http://www.red-bean.com/sgf/proplist_ff.html
        // and SGF file examples
        // http://www.red-bean.com/sgf/examples/

        public string Comment { get; set; }
        public string Name { get; set; }

        public List<string> AddBlack { get; set; }
        public List<string> AddWhite { get; set; }

        /// <summary>
        /// Describes current state of the entire game board
        /// </summary>
        public IntersectionState[,] BoardState { get; set; }

        // Contain territory
        // public List<Shape> Figures { get; set; } - Implement Shape 
        public List<KeyValuePair<Position, string>> Labels { get; set; }

        public List<Move> Moves { get; set; }

        /*
         *  When there is more than one recorded move after a move, 
         *  always branch:
         *  (;W[dd]N[W d16]     // White plays
         *  (;B[pp]N[B q4])     // Black plays - Possible Move A
         *  (;B[dp]N[B d4]))    // Black plays - Possible Move B
        */
        public List<GameTreeNode> Branches { get; set; }
    }
}
