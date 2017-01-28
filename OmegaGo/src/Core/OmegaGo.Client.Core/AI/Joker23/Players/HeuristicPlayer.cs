using System;
using System.Collections.Generic;
using OmegaGo.Core.AI.Joker23.GameEngine;
using OmegaGo.Core.Helpers;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class HeuristicPlayer : JokerPlayer
    {

        private int[,] orderMap;
        private int[] dx = { 1, -1, 0, 0 };
        private int[] dy = { 0, 0, -1, 1 };
        public HeuristicPlayer(char color) : base(color)
        {
        }

        public override void planMove(JokerGame game)
        {
            throw new NotImplementedException();
        }

        public JokerPoint betterPlanMove(JokerGame game)
        {
            if (orderMap == null)
            {
                initOrderMap(game.getHeight());
            }

            int n = game.getHeight();

            JokerMove prevMove;
            if ((prevMove = game.getLastMove()) != null)
            {
                int row = prevMove.getLocation().x;
                int col = prevMove.getLocation().y;

                updateOrderMap(row, col, n);
            }

            char[,] tmpBoard = game.getBoardCopy();
            int best = int.MinValue;
            List<JokerPoint> bestMoves = new List<JokerPoint>();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (tmpBoard[i,j] != '*')
                    {
                        continue;
                    }

                    tmpBoard[i,j] = color;
                    int heuristic = AiUtil.getLiberties(getColor(), tmpBoard);
                    heuristic -= (AiUtil.getLiberties(getOpponentColor(), tmpBoard) * 2);
                    heuristic += orderMap[i,j];

                    if (heuristic > best)
                    {
                        best = heuristic;
                        bestMoves.Clear();
                        bestMoves.Add(new JokerPoint(i, j));
                    }
                    else if (heuristic == best)
                    {
                        bestMoves.Add(new JokerPoint(i, j));
                    }

                    tmpBoard[i,j] = '*';
                }
            }
            

            JokerPoint bestMove = bestMoves[Randomizer.Next(bestMoves.Count)];
            updateOrderMap(bestMove.x, bestMove.y, n);
            return bestMove;
        }

        private void updateOrderMap(int row, int col, int n)
        {
            orderMap[row,col] = 0;
            for (int i = 0; i < dx.Length; i++)
            {
                int nr = row + dx[i];
                int nc = col + dy[i];

                if (nr < 0 || nr >= n || nc < 0 || nc >= n)
                {
                    continue;
                }
                orderMap[nr,nc] *= 2;
            }
        }

        private void initOrderMap(int n)
        {
            this.orderMap = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    orderMap[i, j] = 3;
                }
            }

            for (int i = 0; i < n; i++)
            {
                orderMap[0, i] =
                    orderMap[i, 0] =
                    orderMap[i, n - 1] =
                    orderMap[n - 1, i] = 1;
            }
        }
    }
}