using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

namespace OmegaGo.UI.WindowsUniversal.Services.BoardControl
{
    class RenderUtilities
    {
        public static Rect Scale(Rect target, int originalWidth, int originalHeight)
        {
            double orWidth = originalWidth;
            double orHeight = originalHeight;
            double maxWidth = target.Width;
            double maxHeight = target.Height;
            double xOverflowRatio = orWidth/maxWidth;
            double yOverflowRatio = orHeight/maxHeight;
            if (xOverflowRatio >= yOverflowRatio)
            {
                double xMagnification = 1.0/xOverflowRatio;
                orWidth = maxWidth;
                orHeight = orHeight*xMagnification;
            }
            else
            {
                double yMagnification = 1.0/yOverflowRatio;
                orHeight = maxHeight;
                orWidth = orWidth*yMagnification;
            }
            return new Rect(
                    target.X + target.Width / 2 - orWidth / 2,
                    target.Y + target.Height / 2 - orHeight / 2,
                    orWidth,
                    orHeight
                );
        }

        private static int cachedCellSize = -1;
        private static Dictionary<string, CanvasTextLayout> cache = new Dictionary<string, CanvasTextLayout>();
        public static CanvasTextLayout GetCachedCanvasTextLayout(ICanvasResourceCreator resourceCreator, string text, CanvasTextFormat textFormat, int cellSize)
        {
            if (cellSize != cachedCellSize)
            {
                foreach(var val in cache.Values)
                {
                    val.Dispose();
                }
                cache.Clear();
                cachedCellSize = cellSize;
            }
            if (!cache.ContainsKey(text))
            {
                cache[text]= new CanvasTextLayout(resourceCreator, text, textFormat, cellSize, cellSize);
            }
            return cache[text];
        }
    }
}
