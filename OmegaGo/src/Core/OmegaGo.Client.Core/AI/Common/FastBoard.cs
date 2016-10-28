using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Common
{
    class FastBoard
    {
        public static List<Position> GetAllLegalMoves(Color[,] board)
        {
            // TODO make this work according to rules
            List<Position> legalMoves = new List<Core.Position>();
            for (int i = 0; i < board.GetLength(0); i++)
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == Color.None)
                    {
                        legalMoves.Add(new Core.Position() {X = i, Y = j});
                    }
                }
            return legalMoves;
        }
    }
}
