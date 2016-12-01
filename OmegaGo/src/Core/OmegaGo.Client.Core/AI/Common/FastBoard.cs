using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Common
{
    public class FastBoard
    {
        public static List<Position> GetAllLegalMoves(StoneColor[,] board)
        {
            // TODO make this work according to rules
            List<Position> legalMoves = new List<Core.Position>();
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == StoneColor.None)
                    {
                        legalMoves.Add(new Core.Position() {X = i, Y = j});
                    }
                }
            return legalMoves;
        }
        public static StoneColor[,] CloneBoard(StoneColor[,] board)
        {
            StoneColor[,] newBoard = new StoneColor[board.GetLength(0), board.GetLength(1)];
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    newBoard[i, j] = board[i, j];
                }
            return newBoard;
        }

        public static StoneColor[,] CreateBoardFromGame(Game game)
        {
            StoneColor[,] createdBoard = new StoneColor[game.SquareBoardSize, game.SquareBoardSize];
            foreach (Move move in game.PrimaryTimeline)
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

        public static StoneColor[,] BoardWithoutTheseStones(StoneColor[,] boardState, IEnumerable<Position> deadPositions)
        {
            StoneColor[,] newBoard = FastBoard.CloneBoard(boardState);
            foreach(var position in deadPositions)
            {
                newBoard[position.X, position.Y] = StoneColor.None;
            }
            return newBoard;
        }
    }
}
