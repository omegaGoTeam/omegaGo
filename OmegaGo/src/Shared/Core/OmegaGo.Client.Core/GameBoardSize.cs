using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    public struct GameBoardSize
    {
        /// <summary>
        /// Initializes square game board
        /// </summary>
        /// <param name="size">Size of the game board</param>
        public GameBoardSize( int size )
        {
            Width = size;
            Height = size;
        }

        /// <summary>
        /// Initializes rectangular game board
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public GameBoardSize( int width, int height )
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Width of the game board
        /// </summary>
        public int Width { get; }
        
        /// <summary>
        /// Height of the game board
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Checks if the board is square
        /// </summary>
        public bool IsSquare => Width == Height;
    }
}
