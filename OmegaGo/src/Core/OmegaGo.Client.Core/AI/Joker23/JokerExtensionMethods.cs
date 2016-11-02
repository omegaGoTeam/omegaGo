﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Joker23
{
    public static class JokerExtensionMethods
    {
        public static bool isEmpty<T>(this IList<T> list)
        {
            return list.Count == 0;
        }
        public static bool isEmpty<T>(this LinkedList<T> list)
        {
            return list.Count == 0;
        }

        public static char[,] OurBoardToJokerBoard(Color[,] board, GameBoardSize size)
        {
            char[,] jokerBoard = new char[size.Width, size.Height];
            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    char targetChar = '*';
                    if (board[x, y] == Color.Black) targetChar = 'B';
                    if (board[x, y] == Color.White) targetChar = 'W';
                    jokerBoard[x, y] = targetChar;
                }
            }
            return jokerBoard;
        }
    }
}
