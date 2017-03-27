using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.Utility
{
    public class Rectangle
    {
        public Rectangle(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public static Rectangle GetBoundingRectangle(GameBoard board)
        {
            int x1 = board.Size.Width;
            int y1 = board.Size.Height;
            int x2 = 0;
            int y2 = 0;
            for (int x = 0; x < board.Size.Width; x++)
            {
                for (int y = 0; y < board.Size.Height; y++)
                {
                    bool filled = board[x, y] != StoneColor.None;
                    if (filled)
                    {
                        if (x < x1) x1 = x;
                        if (y < y1) y1 = y;
                        if (x > x2) x2 = x;
                        if (y > y2) y2 = y;
                    }
                }
            }
            int safeSpace = 2;
            // Correction
            x1 = Math.Max(0, x1 - safeSpace);
            y1 = Math.Max(0, y1 - safeSpace);
            x2 = Math.Min(board.Size.Width - 1, x2 + safeSpace);
            y2 = Math.Min(board.Size.Height - 1, y2 + safeSpace);
            int w = x2 - x1 + 1;
            int h = y2 - y1 + 1;
            return new Utility.Rectangle(
                x1,
                y1,
                w,
                h)
                ;
        }
    }
}
