using System;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Represents the size of a Go board. Almost all games of Go are played on a square board. Notably, IGS does not
    /// permit non-squares boards. However, OGS does permit them and various singleplayer Go programs do as well. 
    /// </summary>
    public struct GameBoardSize : IEquatable<GameBoardSize>
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
        public int Width { get; }
        
        /// <summary>
        /// Height of the game board.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Checks if the board is square.
        /// </summary>
        public bool IsSquare => Width == Height;
        
        public static bool operator ==(GameBoardSize first, GameBoardSize second) => first.Equals(second);

        public static bool operator !=(GameBoardSize first, GameBoardSize second) => !(first == second);

        public bool Equals(GameBoardSize other)
        {
            return other.Height == Height && other.Width == Width;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Width * 397) ^ this.Height;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is GameBoardSize && Equals((GameBoardSize)obj);
        }

        public override string ToString()
        {
            return Width + "x" + Height;
        }
    }
}
