using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    /// <summary>
    /// Represents the size of a Go board. Almost all games of Go are played on a square board. Notably, IGS does not
    /// permit non-squares boards. However, OGS does permit them and various singleplayer Go programs do as well. 
    /// </summary>
    public struct GameBoardSize
    {
        /// <summary>
        /// Initializes a square game board.
        /// </summary>
        /// <param name="size">Size of the game board</param>
        public GameBoardSize( int size ) : this()
        {
            Width = size;
            Height = size;
        }

        /// <summary>
        /// Initializes a rectangular game board.
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public GameBoardSize( int width, int height ) : this()
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Width of the game board.
        /// </summary>
        public int Width { get;  }
        
        /// <summary>
        /// Height of the game board.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Checks if the board is square.
        /// </summary>
        public bool IsSquare => Width == Height;
    }
}
