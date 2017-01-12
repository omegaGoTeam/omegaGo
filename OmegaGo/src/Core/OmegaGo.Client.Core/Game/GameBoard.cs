using System;

namespace OmegaGo.Core.Game
{
    public sealed class GameBoard : IEquatable<GameBoard>
    {
        private StoneColor[,] _board;

        public GameBoardSize Size { get; }

        /// <summary>
        /// Initializes a new <see cref="GameBoard"/> with the specified dimensions.
        /// </summary>
        /// <param name="boardSize">Size of the board.</param>
        public GameBoard(GameBoardSize boardSize)
        {
            _board = new StoneColor[boardSize.Width, boardSize.Height];
            this.Size = boardSize;
        }

        /// <summary>
        /// Initializes a new <see cref="GameBoard"/> as a copy of the given game board.
        /// </summary>
        /// <param name="gameBoard">The game board to copy.</param>
        public GameBoard(GameBoard gameBoard)
            : this(gameBoard.Size)
        {
            for (int x = 0; x < Size.Width; x++)
            {
                for (int y = 0; y < Size.Height; y++)
                {
                    _board[x, y] = gameBoard[x, y];
                }
            }
        }



        public StoneColor this[Position position]
        {
            get
            {
                return this[position.X, position.Y];
            }
            set
            {
                this[position.X, position.Y] = value;
            }
        }

        public StoneColor this[int x, int y]
        {
            get
            {
                return _board[x, y];
            }
            set
            {
                _board[x, y] = value;
            }
        }

        public bool Equals(GameBoard other)
        {
            if ( other != null)
            {
                if (other.Size == Size)
                {
                    for (int x = 0; x < Size.Width; x++)
                    {
                        for (int y = 0; y < Size.Height; y++)
                        {
                            if (other[x, y] != this[x, y]) return false;
                        }                        
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
