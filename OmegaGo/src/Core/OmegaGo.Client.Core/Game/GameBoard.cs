using System;
using System.Collections.Generic;
using System.Linq;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Represents the game board
    /// </summary>
    public sealed class GameBoard : IEquatable<GameBoard>
    {
        /// <summary>
        /// Board array
        /// </summary>
        private readonly StoneColor[,] _board;

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

        /// <summary>
        /// Size of the game board
        /// </summary>
        public GameBoardSize Size { get; }

        public static bool operator ==(GameBoard first, GameBoard second) => Equals(first, second);

        public static bool operator !=(GameBoard first, GameBoard second) => !(first == second);

        /// <summary>
        /// Creates a game board from a Game Tree Node
        /// </summary>
        /// <param name="gameInfo"></param>
        /// <param name="gameTree">Game tree</param>
        /// <returns></returns>
        public static GameBoard CreateBoardFromGameTree(GameInfo gameInfo, GameTree gameTree)
        {
            GameBoard createdBoard = new GameBoard(gameInfo.BoardSize);
            foreach (Move move in gameTree.PrimaryMoveTimeline)
            {
                if (move.Kind == MoveKind.PlaceStone)
                {
                    createdBoard[move.Coordinates.X, move.Coordinates.Y] = move.WhoMoves;
                }
                foreach (Position p in move.Captures)
                {
                    createdBoard[p.X, p.Y] = StoneColor.None;
                }
            }
            return createdBoard;
        }

        /// <summary>
        /// Gets or sets the stone at a given position
        /// </summary>
        /// <param name="position">Position</param>
        /// <returns>Stone color</returns>
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

        /// <summary>
        /// Gets or sets the stone at a given position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Stone color</returns>
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

        /// <summary>
        /// Returns new board without given stones
        /// </summary>
        /// <param name="deadPositions"></param>
        /// <returns>New board without given stones</returns>
        public GameBoard BoardWithoutTheseStones( IEnumerable<Position> deadPositions)
        {
            GameBoard newBoard = new GameBoard(this);
            foreach (var position in deadPositions)
            {
                newBoard[position.X, position.Y] = StoneColor.None;
            }
            return newBoard;
        }

        /// <summary>
        /// Tests equality of two game boards
        /// </summary>
        /// <param name="other">Game board to compare</param>
        /// <returns>Are game boards equal?</returns>
        public bool Equals(GameBoard other)
        {
            if (other != null)
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

        public override int GetHashCode() => Size.GetHashCode();

        public override bool Equals(object obj) => Equals(obj as GameBoard);

        public override string ToString()
        {
            return Size.ToString() + " (" + this.AllPositions.Count() + " filled)";
        }

        private IEnumerable<StoneColor> AllPositions
        {
            get
            {
                for (int x = 0; x < Size.Width; x++)
                {
                    for (int y =0; y < Size.Height; y++)
                    {
                        yield return this[x, y];
                    }
                }
            }
        }
    }
}
