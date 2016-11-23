using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    public sealed class GameBoard : IEquatable<GameBoard>
    {
        private StoneColor[,] _board;
        private GameBoardSize _size;

        public GameBoardSize Size => _size;
        
        public GameBoard(GameBoardSize boardSize)
        {
            _board = new StoneColor[boardSize.Width, boardSize.Height];
            _size = boardSize;
        }

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
