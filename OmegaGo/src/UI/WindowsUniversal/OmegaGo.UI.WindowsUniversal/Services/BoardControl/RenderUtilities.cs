using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using OmegaGo.UI.WindowsUniversal.Services.Game;

namespace OmegaGo.UI.WindowsUniversal.Services.BoardControl
{
    /// <summary>
    /// Contains some useful functions for <see cref="RenderService"/>.
    /// </summary>
    static class RenderUtilities
    {
        /// <summary>
        /// Returns a rectangle that exactly fits the specified target rectangle. The width/height ratio
        /// given as parameters is maintained. The returned rectangle will have either its width or height
        /// equal to that of the target rectangle.
        /// </summary>
        /// <param name="target">The target rectangle that we want to fit in..</param>
        /// <param name="originalWidth">Width of the rectangle that we want to scale to fit.</param>
        /// <param name="originalHeight">Height of the rectangle that we want to scale to fit.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets a <see cref="CanvasTextLayout"/> created with the specified parameters. Maintains a cache
        /// of results, indexed by <paramref name="text"/>, for as long as the <paramref name="cellSize"/> doesn't change.
        /// </summary>
        /// <param name="resourceCreator">The resource creator.</param>
        /// <param name="text">The text.</param>
        /// <param name="textFormat">The text format to apply.</param>
        /// <param name="cellSize">Used as both width and height of the result.</param>
        /// <returns></returns>
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
