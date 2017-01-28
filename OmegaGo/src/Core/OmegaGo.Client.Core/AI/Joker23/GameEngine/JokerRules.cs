using System;
using System.Collections.Generic;

namespace OmegaGo.Core.AI.Joker23.GameEngine
{

    public class Rules
    {

        public static bool isMoveLegal(char color, int row, int col, char[,] board)
        {
            if (board[row, col] != '*')
            {
                return false;
            }


            return true;
        }

        public static bool gameOver(char[,] board, int height, int width)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (board[i, j] == '*')
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static String serializeBoardState(char[,] board, int height, int width)
        {

            String boardString = "";

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    boardString = boardString + board[i, j];
                }
            }
            return boardString;
        }

        public static char[,] copyBoard(char[,] board, int height, int width)
        {

            char[,] boardCopy = new char[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    boardCopy[i, j] = board[i, j];
                }
            }
            return boardCopy;
        }

        public static char getWinner(char[,] board, int height, int width)
        {
            int w = 0;
            int b = 0;

            int n = height;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (board[i,j] == 'W')
                    {
                        w++;
                    }

                    if (board[i,j] == 'B')
                    {
                        b++;
                    }
                }
            }

            if (w > b + 1)
            {
                return 'W'; //white wins
            }
            else if (b > w + 1)
            {
                return 'B'; //black wins
            }
            else
            {
                return 'N'; //no one wins
            }
        }

        private static bool[,] vis;
        private static int previousVisSize = -1;
        private static int[] dx = { 1, -1, 0, 0 };
        private static int[] dy = { 0, 0, -1, 1 };
        private static char[,] board;

        public static LinkedList<JokerPoint> findCaptured(char wall,
            char indanger,
            char[,] input,
            int height)
        {
            // Takes 62% CPU.
            if (height != previousVisSize)
            {
                vis = new bool[height, height];
                previousVisSize = height;
            }
            else
            {
                for (int x = 0; x < height; x++)
                    for (int y= 0 ;y < height;y++)
                    {
                        vis[x, y] = false;
                    }
            }

            board = input;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (board[i, j] == '*' && !vis[i, j])
                    {
                        dfs(i, j, height, wall);
                    }
                }
            }

            LinkedList<JokerPoint> ret = new LinkedList<JokerPoint>();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (board[i, j] == indanger && !vis[i, j])
                    {
                        ret.AddLast(new JokerPoint(i, j));
                    }
                }
            }

            return ret;
        }

        private static void dfs(int row, int col, int n, char wall)
        {
            if (row < 0 || row >= n || col < 0 || col >= n || board[row, col] == wall || vis[row, col])
            {
                return;
            }

            vis[row, col] = true;

            for (int i = 0; i < dx.Length; i++)
            {
                int r1 = row + dx[i];
                int c1 = col + dy[i];

                dfs(r1, c1, n, wall);
            }
        }
    }
}