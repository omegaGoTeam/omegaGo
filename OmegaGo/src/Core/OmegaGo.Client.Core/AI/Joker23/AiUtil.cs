using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Extensions;
// ReSharper disable All

namespace OmegaGo.Core.AI.Joker23
{
    public class AiUtil
    {

        private static bool[,] vis;
        private static char[,] board;
        private static int boardWidth = -1;
        private static int[] dr = { 1, -1, 0, 0 };
        private static int[] dc = { 0, 0, -1, 1 };

        public static int getLiberties(char color, char[,] input)
        {
            // Takes 15% CPU
            board = input;
            int width = board.GetLength(0);
            if (width != boardWidth)
            {
                vis = new bool[width, width];
            }
            else
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y< width;y++)
                    {
                        vis[x, y] = false;
                    }
                }
            }
            boardWidth = width;

            int ret = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (board[i, j] == color)
                    {
                        ret += countAround(i, j);
                    }
                }
            }
            return ret;
        }

        private static int countAround(int r, int c)
        {
            // Takes 8% CPU.
            int ret = 0;
            for (int i = 0; i < dr.Length; i++)
            {
                int nextr = r + dr[i];
                int nextc = c + dc[i];

                if (nextr < 0 || nextr >= boardWidth || nextc < 0 || nextc >= boardWidth || vis[nextr, nextc])
                {
                    continue;
                }

                if (board[nextr, nextc] == '*')
                {
                    ret++;
                    vis[nextr, nextc] = true;
                }
            }
            return ret;
        }


        public static IEnumerable<JokerPoint> getNextMoves(char[,] input, int horizontalPruning)
        {
            // Takes 10% CPU
            int n = input.GetLength(0);

            List<InnerMove> pq = new List<InnerMove>(90); // greater initial capacity

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (input[i, j] == '*')
                    {
                        pq.Add(new InnerMove(new JokerPoint(i, j), countAroundBlank(input, i, j)));
                    }
                }
            }

            pq.Shuffle();
            pq.Sort();

            List<JokerPoint> ret = new List<JokerPoint>();

            int upTo = 0;
            while (upTo < pq.Count && pq[upTo].influence > 0)
            {
                upTo++;
            }
            while (upTo < pq.Count && horizontalPruning-- > 0)
            {
                upTo++;
            }
            return pq.Take(upTo).Select(move => move.move);
        }

        private static int countAroundBlank(char[,] input, int r, int c)
        {
            int ret = 0;
            for (int i = 0; i < dr.Length; i++)
            {
                int nextr = r + dr[i];
                int nextc = c + dc[i];

                if (nextr < 0 || nextr >= boardWidth || nextc < 0 || nextc >= boardWidth)
                {
                    continue;
                }

                if (input[nextr, nextc] != '*')
                {
                    ret++;
                }
            }
            return ret;
        }
        /*
    public static List<Point> getNextMoves(char[][] board, int[][] influence, int horizontalPruning) {
        int n = board.length;
        LinkedList<Move> pq = new LinkedList<Move>();
        for(int i=0; i<n; i++) {
            for(int j=0; j<n; j++) {
                if(board[i][j] == '*') {
                    pq.add(new Move(new Point(i, j), influence[i][j]));
                }
            }
        }

        Collections.shuffle(pq);
        Collections.sort(pq);

        List<Point> ret = new LinkedList<Point>();
        while(!pq.isEmpty() && horizontalPruning --> 0) {
            ret.add(pq.poll().move);
        }

        return ret;
    }
    */
        private struct InnerMove : IComparable<InnerMove>
        {
            public JokerPoint move;
            public int influence;

            public InnerMove(JokerPoint move, int influence)
            {
                this.move = move;
                this.influence = influence;
            }

            public int CompareTo(InnerMove other)
            {
                return other.influence - this.influence;
            }
        }
    }
}