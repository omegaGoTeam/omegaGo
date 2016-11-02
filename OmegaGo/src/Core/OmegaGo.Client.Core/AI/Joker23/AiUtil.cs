﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Joker23
{
    public class AiUtil
    {

        private static bool[,] vis;
        private static char[,] board;
        private static int boardWidth;
        private static int[] dr = { 1, -1, 0, 0 };
        private static int[] dc = { 0, 0, -1, 1 };

        public static int getLiberties(char color, char[,] input)
        {
            board = input;
            int width = board.GetLength(0);
            boardWidth = width;
            vis = new bool[width, width];

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

        /*
    public static List<JokerPoint> getNextMoves(char[,] input, int horizontalPruning) {
        int n = input.Length;

            List<InnerMove> pq = new List<InnerMove>();

        for(int i=0; i<n; i++) {
            for(int j=0; j<n; j++) {
                if(input[i,j] == '*'){
                    pq.Add(new InnerMove(new JokerPoint(i, j), countAroundBlank(input, i, j)));
                }
            }
        }

        CollectionsShuffle(pq);
        Collections.sort(pq);

        List<JokerPoint> ret = new LinkedList<JokerPoint>();

        while(!pq.isEmpty() && pq.peek().influence > 0) {
            ret.add(pq.poll().move);
        }

        while(!pq.isEmpty() && horizontalPruning --> 0) {
            ret.add(pq.poll().move);
        }

        return ret;
    }
    */
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

    private static class Move implements Comparable<Move> {
        Point move;
        int influence;

        public Move(Point move, int influence) {
            this.move = move;
            this.influence = influence;
        }

        public int compareTo(Move m) {
            return m.influence - this.influence;
        }
    }*/
    }
}