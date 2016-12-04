using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Common
{
    public class FastBoard
    {
        public static List<Position> GetAllLegalMoves(GameBoard board)
        {
            // TODO make this work according to rules
            List<Position> legalMoves = new List<Core.Position>();
            for (int i = 0; i < board.Size.Width; i++)
                for (int j = 0; j < board.Size.Height; j++)
                {
                    if (board[i, j] == StoneColor.None)
                    {
                        legalMoves.Add(new Core.Position() {X = i, Y = j});
                    }
                }
            return legalMoves;
        }
        
        public static GameBoard CreateBoardFromGame(GameInfo game)
        {
            GameBoard createdBoard = new GameBoard(game.BoardSize);
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

        public static GameBoard BoardWithoutTheseStones(GameBoard boardState, IEnumerable<Position> deadPositions)
        {
            GameBoard newBoard = new GameBoard(boardState);
            foreach(var position in deadPositions)
            {
                newBoard[position.X, position.Y] = StoneColor.None;
            }
            return newBoard;
        }
    }
}
