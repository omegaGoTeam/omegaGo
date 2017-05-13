using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23
{
    /// <summary>
    /// This class adds some Java methods to C# classes and contains auxiliary methods for interopability.
    /// </summary>
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

        public static char[,] OurBoardToJokerBoard(GameBoard board, GameBoardSize size)
        {
            char[,] jokerBoard = new char[size.Width, size.Height];
            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    char targetChar = '*';
                    if (board[x, y] == StoneColor.Black) targetChar = 'B';
                    if (board[x, y] == StoneColor.White) targetChar = 'W';
                    jokerBoard[x, y] = targetChar;
                }
            }
            return jokerBoard;
        }
        public static void ArraysFill<T>(T[] array, T val)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = val;
            }
        }
    }
}
